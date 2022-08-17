using Helpdesk_Backend_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, string connectionString)
        {
            return
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        }
    }
}
