﻿using CloudinaryDotNet;
using Dumps.Application.ServicesInterfaces;
using Dumps.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dumps.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Get Cloudinary configuration from the configuration object
            var cloudinarySettings = configuration.GetSection("Cloudinary");

            // Register Cloudinary with the configuration
            services.AddSingleton(new Cloudinary(new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            )));
            // Register infrastructure services
            // services.AddScoped<IExternalApiService, ExternalApiService>();
            // services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IStorageService, CloudinaryStorageService>();

            return services;
        }
    }
}
