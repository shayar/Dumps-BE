using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dumps.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Configuration of MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            // Register any application-specific services here
            // services.AddScoped<IApplicationService, ApplicationService>();

            return services;
        }
    }
}
