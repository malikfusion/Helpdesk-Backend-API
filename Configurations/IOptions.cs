using Helpdesk_Backend_API.Entities.NonDbEntities;

namespace Helpdesk_Backend_API.Configurations
{
    public static class IOptions
    {
        public static IServiceCollection ConfigureAppSetting(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<JWT>(configuration.GetSection("JWT"));
        }
    }
}
