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

            await SeedMajorsAsync(context);
        }

        private static async Task SeedMajorsAsync(AppDbContext context)
        {
            if (!context.Majors.Any())
            {
                var majors = new List<Major>
                {
                    new Major { Id = new Guid("a1b2c3d4-e5f6-7890-1234-56789abcdef0"), Name = "Information Technology", Code = "IT" },
                    new Major { Id = new Guid("b2c3d4e5-f678-9012-3456-789abcdef012"), Name = "Economic", Code = "E" },
                    new Major { Id = new Guid("c3d4e5f6-7890-1234-5678-9abcdef01234"), Name = "Language", Code = "L" }
                };

                await context.Majors.AddRangeAsync(majors);

                await context.SaveChangesAsync();

                Console.WriteLine("Seeded Majors:");

                foreach (var major in majors)
                {
                    Console.WriteLine($"ID: {major.Id}, Name: {major.Name}");
                }

                // Construct the Kafka event
                //var majorEvent = new MajorSeededEventDTO
                //{
                //    Data = new MajorSeededEventData
                //    {
                //        Majors = majors.Select(m => new MajorSingleData
                //        {
                //            Id = m.Id,
                //            Name = m.Name
                //        }).ToList()
                //    }
                //};
            }
        }
    }
}
