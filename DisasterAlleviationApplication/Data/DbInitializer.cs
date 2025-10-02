using Microsoft.AspNetCore.Identity;
using DisasterAlleviationApplication.Models;

namespace DisasterAlleviationApplication.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create Admin Role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create User Role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Check if admin user already exists
            var adminEmail = "admin@disasterrelief.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create admin user
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Create user with password
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // Assign Admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin user created successfully!");
                    Console.WriteLine($"Email: {adminEmail}");
                    Console.WriteLine("Password: Admin@123");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                // Ensure existing admin has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin role assigned to existing user.");
                }
            }
        }
    }
}
