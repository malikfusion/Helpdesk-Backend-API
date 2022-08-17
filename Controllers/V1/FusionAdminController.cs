using AutoMapper;
using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.DTOs.Consts;
using Helpdesk_Backend_API.Entities;
using Helpdesk_Backend_API.Services;
using Helpdesk_Backend_API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IFusionAdminService fusionAdminService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AdminController(IFusionAdminService fusionAdminService, IMapper mapper, IUserService userService)
        {
            this.fusionAdminService = fusionAdminService;
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin(CreateFusionAdminDto model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            if (await userService.IsEmailTaken(model.Email, token))
            {
                ModelState.AddModelError("Invalid", "Email is already taken");
                return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var generatedId = Guid.NewGuid().ToString();

            var adminToAdd = new FusionAdmin()
            {
                Id = generatedId,
                UserId = generatedId,
                User = new HelpDeskUser()
                {
                    Id = generatedId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    UserName = model.Email,
                    RoleName = Roles.Admin
                }
            };

            using (var dbSession = await userService.BeginTransactionAsync())
            {
                try
                {

                    var userResult = await userService.CreateUser(adminToAdd.User);
                    if (userResult == null)
                    {
                        ModelState.AddModelError("Failed", "Failed To Create Admin");
                        return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                    }

                    var result = await fusionAdminService.Create(adminToAdd, token);
                    if (result == null)
                    {
                        ModelState.AddModelError("Failed", "Failed To Create Admin");
                        return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
                    }

                    await dbSession.CommitAsync();

                    var adminResult = await fusionAdminService.GetFusionAdmin(result.Id, token); 

                    return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetFusionAdminDto>(adminResult)));
                }
                catch (Exception e)
                {
                    await dbSession.RollbackAsync();
                    ModelState.AddModelError("BadRequest", e.Message ?? e.InnerException.Message);
                    return BadRequest(ResponseBuilder.BuildResponse<object>(/*400, "BadRequest",*/ ModelState, null));
                }
            }
        }
    

        [HttpGet("get-admin")]
        public async Task<IActionResult> GetAdminAccount(string adminId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(adminId))
            {
                ModelState.AddModelError("Invalid", "Please Input a valid string for adminId");
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var admin = await fusionAdminService.GetFusionAdmin(adminId, token);

            if(admin is null)
            {
                ModelState.AddModelError("NotFound", "Admin not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetFusionAdminDto>(admin)));
        }


        [HttpGet("list-admins")]
        public async Task<IActionResult> ListAdminAccount(int page, int perPage, CancellationToken token)
        {
            var fusion = fusionAdminService.ListAllFusionAdmins();

            if(fusion.Count() == 0)
            {
                return Ok(ResponseBuilder.BuildResponse<object>(null, null));
            }

            var mappedData = mapper.Map<List<GetFusionAdminDto>>(fusion.Paginate(page, perPage));

            var pagedData = Pagination.GetPagedData(mappedData, page, perPage, await fusion.CountAsync(token));

            return Ok(ResponseBuilder.BuildResponse(null, pagedData));
        }



        [HttpPatch("update-admin-account")]
        public async Task<IActionResult> UpdatefusionAccount([Required]string adminId, UpdateFusionAdminDto model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            var admin = await fusionAdminService.GetFusionAdmin(adminId, token);

            if(admin is null)
            {
                ModelState.AddModelError("NotFound", "admin account not found");
                return NotFound(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            admin = mapper.Map(model, admin);

            var updateResult = await fusionAdminService.Update(admin, token);

            if(updateResult is null)
            {
                ModelState.AddModelError("Failed", "Failed to update admin account");
                return UnprocessableEntity(ResponseBuilder.BuildResponse<object>(ModelState, null));
            }

            return Ok(ResponseBuilder.BuildResponse(null, mapper.Map<GetFusionAdminDto>(admin)));
        }


    }
}
