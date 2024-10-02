using System.Reflection;
using Dumps.Application.Command;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dumps.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Configuration of MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<
                    CreateContactUsCommand>(); // registers validators from Assembly where CreateContactUsCommand is locatedinitia
            // Register any application-specific services here
            // services.AddScoped<IApplicationService, ApplicationService>();

            return services;
        }
    }
}
