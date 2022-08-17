using Helpdesk_Backend_API.Services;

namespace Helpdesk_Backend_API.Configurations
{
    public static class ScopedServices
    {
        public static IServiceCollection AddScopedServices(this IServiceCollection services) => services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IRepositoryService, RepositoryService>()
            .AddScoped<IOrganizationService, OrganizationService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IProjectService, ProjectService>()
            .AddScoped<IFusionAdminService, FusionAdminService>()
            .AddScoped<IStaffService, StaffService>()
            .AddScoped<ITicketService, TicketService>();
    }
}
