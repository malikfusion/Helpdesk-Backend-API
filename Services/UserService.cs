using AutoMapper;
using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Helpdesk_Backend_API.Entities.NonDbEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Services
{
    public interface IUserService
    {
        public SignInManager<HelpDeskUser> SignInManager { get; }
        public UserManager<HelpDeskUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        Task<AuthResponse> GenerateToken(HelpDeskUser user, RefreshToken existingRefreshToken);
        Task<AuthResponse> RefreshUserToken(GenerateRefreshTokenDto model);
        Task<bool> RevokeUserToken(string userId);
        
        ValueTask<AuthResponse> Login(LoginRequestModel request);

        Task<string> GetEmailConfirmationToken(HelpDeskUser user);

        Task<HelpDeskUser> CreateUser<T>(T userDto) where T : HelpDeskUser; 
        Task<HelpDeskUser> GetUser(string userId, CancellationToken token);

        Task<bool> UpdateUser(HelpDeskUser user);
        IQueryable<HelpDeskUser> ListAllUsers(CancellationToken token);
        Task<bool> ValidatePassword(HelpDeskUser helpdeskUser, string password);
        Task<bool> DeleteUser(HelpDeskUser user, CancellationToken token);
        ValueTask<IDbContextTransaction> BeginTransactionAsync();
        Task<bool> IsEmailTaken(string email, CancellationToken token);

    }

    public class UserService : IUserService
    {
        private readonly IRepositoryService repository;
        private readonly IMapper Mapper;
        private readonly JWT _jwt;
        public SignInManager<HelpDeskUser> SignInManager { get; }
        public UserManager<HelpDeskUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public UserService(IRepositoryService repositoryService, RoleManager<IdentityRole> roleManager, SignInManager<HelpDeskUser>
            signInManager, UserManager<HelpDeskUser> userManager,IMapper mapper, IOptions<JWT> options)
        {
            this.Mapper = mapper;
            RoleManager = roleManager;
            SignInManager = signInManager;
            UserManager = userManager;
            _jwt = options.Value;
            this.repository = repositoryService;
            
        }

        public async Task<HelpDeskUser> CreateUser<T>(T userDto) where T : HelpDeskUser
        {
            var user = await UserManager.FindByEmailAsync(userDto.Email);

            if (user is not null)
            {
                throw new Exception("User already exists.");
            }

            var userContact = Mapper.Map<HelpDeskUser>(userDto);

            if (await RoleManager.FindByNameAsync(userDto.RoleName) == null)
            {
                throw new Exception("This role does not exist");
            }

            var result = await UserManager.CreateAsync(userContact, userDto.Password);

            if (result.Succeeded)
            {

                HelpDeskUser contact = new HelpDeskUser();

                if (userDto.Email is null)
                {
                    contact = await UserManager.FindByNameAsync(userDto.UserName);
                }

                else
                {
                    contact = await UserManager.FindByEmailAsync(userDto.Email);
                }

                var addToRole = await UserManager.AddToRoleAsync(contact, contact.RoleName);

                if (!addToRole.Succeeded)
                {
                    throw new Exception(string.Join(",", addToRole.Errors.Select(c => c.Description)));
                }

                return contact;

            }

            throw new Exception("Error Creating User:" + result.Errors.Select(c => c.Description).First());

        }

        public async Task<AuthResponse> GenerateToken(HelpDeskUser user, RefreshToken existingRefreshToken)
        {
            var key = Encoding.ASCII.GetBytes(_jwt.SigningKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim("userName", user.UserName)
            };

            var role = await RoleManager.Roles.FirstOrDefaultAsync(c => c.Name == user.RoleName);

            var roleClaims = await RoleManager.GetClaimsAsync(role);

            if (roleClaims?.Count > 0)
            {
                claims.AddRange(roleClaims.Select(claim => new Claim(ClaimTypes.Role, claim.Value)));
            }

            var token = CreateToken(claims);
            var createdToken = tokenHandler.CreateToken(token);
            var refreshToken = GenerateRefreshToken();

            _ = int.TryParse(_jwt.RefreshTokenValidityInDays, out int refreshTokenValidityInDays);

            user.RefreshTokenKey = refreshToken;
            user.RefreshTokenExpirytime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            return new AuthResponse()
            {
                ExpiresAt = createdToken.ValidTo,
                Id = user.Id,
                Token = tokenHandler.WriteToken(createdToken),
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> RefreshUserToken(GenerateRefreshTokenDto model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = GetPrincipalFromExpiredToken(model.CurrentJWT);
            if(principal == null)
            {
                throw new Exception("Invalid token");
            }


            var user = await UserManager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null || user.RefreshTokenKey != model.RefreshToken || user.RefreshTokenExpirytime <= DateTime.Now)
            {
                throw new Exception("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var createdToken = tokenHandler.CreateToken(newAccessToken);

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokenKey = newRefreshToken;
            await UserManager.UpdateAsync(user);
            var helpDeskUser = (await repository.FindAsync<HelpDeskUser>(user.Id));

            return new AuthResponse()
            {
                ExpiresAt = createdToken.ValidTo,
                Id = user.Id,
                RefreshToken = newRefreshToken,
                Token = tokenHandler.WriteToken(createdToken),
            };
        }

        public async Task<bool> RevokeUserToken(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new KeyNotFoundException("User Not Found");
            }

            user.RefreshTokenKey = null;

            var updateResult = await UserManager.UpdateAsync(user);

            if (updateResult.Succeeded)
                return true;

            return false;
        }

        public async ValueTask<AuthResponse> Login(LoginRequestModel request)
        {

            var user = await UserManager.FindByEmailAsync(request.UserName) ?? await UserManager.FindByNameAsync(request.UserName);

            if (user is null)
            {
                throw new KeyNotFoundException("Email/Password Not Found");
            }

            
            if (await UserManager.CheckPasswordAsync(user, request.Password))
            {
                return await GenerateToken(user, null);
            }
            
            throw new Exception($"Login failed for user {request.UserName}");

        }

        public async Task<string> GetEmailConfirmationToken(HelpDeskUser user)
        {
            if (user is null)
                return null;

            return await UserManager.GenerateEmailConfirmationTokenAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken)
            {
                throw new SecurityTokenException("Invalid token");
            }
            else if(!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;

        }

        public async Task<HelpDeskUser> GetUser(string userId, CancellationToken token)
        {
            return await UserManager.FindByIdAsync(userId);
        }

         

        public async Task<bool> UpdateUser(HelpDeskUser helpdeskUser)
        {
            if (helpdeskUser == null)
                return false;

            var updateResult = await UserManager.UpdateAsync(helpdeskUser);

            return updateResult.Succeeded;
        }

        public IQueryable<HelpDeskUser> ListAllUsers(CancellationToken token)
        {
            return repository.ListAll<HelpDeskUser>();
        }

        public async Task<bool> ValidatePassword(HelpDeskUser helpdeskUser, string password)
        {
            if (helpdeskUser is null)
                return false;

            return await UserManager.CheckPasswordAsync(helpdeskUser, password);
        }

        public async Task<bool> DeleteUser(HelpDeskUser user, CancellationToken token)
        {
            return await repository.DeleteAsync(user, token);
        }

        private SecurityTokenDescriptor CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
            _ = int.TryParse(_jwt.TokenValidityInMinutes, out int tokenValidityInMinutes);

            var jwtToken = new SecurityTokenDescriptor()
            {
                Audience = _jwt.Audience,
                Issuer = _jwt.Issuer,
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.Now.AddMinutes(tokenValidityInMinutes),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            //var token = new JwtSecurityToken(
            //    issuer: _jwt.Issuer,
            //    audience: _jwt.Audience,
            //    expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
            //    claims: authClaims,
            //    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
            //    );

            return jwtToken;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }



        //private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false,
        //        ValidateIssuer = false,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey)),
        //        ValidateLifetime = false
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        //    if (securityToken is not JwtSecurityToken jwtSecurityToken)
        //    {
        //        throw new SecurityTokenException("Invalid token");
        //    }
        //    else if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        throw new SecurityTokenException("Invalid token");
        //    }


        //    //if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
        //    //    throw new SecurityTokenException("Invalid token");

        //    return principal;

        //}

        //public async Task<string> GetEmailConfirmationToken(HelpDeskUser user)
        //{
        //    if (user is null)
        //        return null;

        //    return await UserManager.GenerateEmailConfirmationTokenAsync(user);
        //}

        //public async Task<bool> ValidatePassword(HelpDeskUser helpdeskUser, string password)
        //{
        //    if (helpdeskUser is null)
        //        return false;

        //    return await UserManager.CheckPasswordAsync(helpdeskUser, password);
        //}

        public ValueTask<IDbContextTransaction> BeginTransactionAsync()
        {
            return repository.BeginTransactionAsync();
        }

        public async Task<bool> IsPhoneNumberTaken(string phoneNumber, CancellationToken token)
        {
            return await repository.ListAll<HelpDeskUser>()
                .AnyAsync(c => c.PhoneNumber == phoneNumber, token);
        }

        public async Task<bool> IsEmailTaken(string email, CancellationToken token)
        {
            return await repository.ListAll<HelpDeskUser>()
                .AnyAsync(c => c.Email == email, token);
        }
    }
}