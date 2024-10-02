using System.Reflection;
using Dumps.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dumps.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register infrastructure services
            // services.AddScoped<IExternalApiService, ExternalApiService>();
            // services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
