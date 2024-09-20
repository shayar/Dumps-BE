using Dumps.Domain.Common.Constants;
using Dumps.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Dumps.Persistence.SeedData
{
    public class SeedData
    {
        public static async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define roles
            var roles = new[] { SD.Role_Admin, SD.Role_User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var newRole = new ApplicationRole(role)
                    {
                        CreatedBy = "system",
                        UpdatedBy = "system",
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    await roleManager.CreateAsync(newRole);
                }
            }

            // Create default admin user
            var adminEmail = SD.AdminEmail;
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var username = SD.Role_Admin;
                var password = SD.AdminPassword;
                if (env == "Production")
                {
                    username  = username + "-live";
                    var livePassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
                    password = string.IsNullOrEmpty(livePassword) ? password : livePassword;
                }
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = adminEmail,
                    FirstName = SD.Role_Admin,
                    LastName = SD.Role_User
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, SD.Role_Admin);
                }
            }
        }
    }
}
