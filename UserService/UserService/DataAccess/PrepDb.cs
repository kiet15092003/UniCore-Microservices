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

                await SeedDataAsync(context, roleManager, isProduction);
            }
        }

        private static async Task SeedDataAsync(AppDbContext context, RoleManager<IdentityRole> roleManager, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--- Applying migrations ---");
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--- Apply migration failed: {ex.Message} ---");
                }
            }

            await SeedRolesAsync(roleManager);
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
    }
}
