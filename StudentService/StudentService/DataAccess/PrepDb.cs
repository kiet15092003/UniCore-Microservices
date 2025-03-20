using Microsoft.EntityFrameworkCore;
using StudentService.Entities;

namespace StudentService.DataAccess
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

            await SeedBatchesAsync(context);
        }
        private static async Task SeedBatchesAsync(AppDbContext context)
        {
            if (!context.Batches.Any())
            {
                var batches = new List<Batch>
                {
                    new Batch { Title = "K22", StartYear = 2022 },
                    new Batch { Title = "K23", StartYear = 2023 },
                    new Batch { Title = "K24", StartYear = 2024 },
                    new Batch { Title = "K25", StartYear = 2025 },
                };

                await context.Batches.AddRangeAsync(batches);

                await context.SaveChangesAsync();

                Console.WriteLine("Seeded Batches:");

                foreach (var batch in batches)
                {
                    Console.WriteLine($"ID: {batch.Id}, Name: {batch.Title}");
                }
            }
        }
    }
}
