using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserService.DataAccess
{
    public static class PrepDb
    {
        public static async Task PrepPopulationAsync(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await SeedDataAsync(context, roleManager, userManager, isProduction);
            }
        }

        private static async Task SeedDataAsync(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, bool isProduction)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedDefaultUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Lecturer", "Student", "TrainingManager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));

                    Console.WriteLine($"Seeded role: {role}");
                }
            }
        }
        
        private static async Task SeedDefaultUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Admin account
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    PersonId = "ADMIN001", // Adding required PersonId
                    Dob = new DateTime(1990, 1, 1), // Default date of birth    
                    Status = 1, // Active status,
                    PhoneNumber = "1234567890" // Default phone number
                };
                
                var result = await userManager.CreateAsync(admin, "Admin123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine("Seeded admin account");
                }
                else
                {
                    Console.WriteLine($"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
