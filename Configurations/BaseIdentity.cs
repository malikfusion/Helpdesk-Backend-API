using Helpdesk_Backend_API.Data;
using Helpdesk_Backend_API.Entities;
using Microsoft.AspNetCore.Identity;

namespace Helpdesk_Backend_API.Configurations
{
    public static class BaseIdentity
    {
        public static IdentityBuilder ConfigureDefaultIdentity(this IServiceCollection services) =>
                   services.AddIdentity<HelpDeskUser, IdentityRole>(options =>
                   {
                       options.Password.RequireLowercase = false;
                       options.Password.RequireUppercase = false;
                       options.Password.RequireNonAlphanumeric = false;
                       options.Password.RequireDigit = false;
                       options.Password.RequiredUniqueChars = 0;

                       options.SignIn.RequireConfirmedAccount = false;
                       options.SignIn.RequireConfirmedEmail = false;
                       options.User.RequireUniqueEmail = true;
                   })
                   .AddEntityFrameworkStores<AppDbContext>()
                   .AddDefaultTokenProviders();
    }
}

