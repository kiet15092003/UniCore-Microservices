using MajorService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MajorService.DataAccess
{
    public static class PrepDb
    {
        public static async Task PrepPopulationAsync(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await SeedDataAsync(context, isProduction);
            }
        }

        private static async Task SeedDataAsync(AppDbContext context, bool isProduction)
        {
            await context.Database.MigrateAsync();
        }
    }
}
