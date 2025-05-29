using CourseService.DataAccess;
using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();
            
            // Seed shifts if they don't exist
            await SeedShiftsAsync(context);
        }
        
        private static async Task SeedShiftsAsync(AppDbContext context)
        {
            if (!await context.Shifts.AnyAsync())
            {
                var shifts = new List<Shift>
                {
                    new Shift
                    {
                        Id = Guid.Parse("dba670c2-255d-40b5-afa5-135466d294fd"),
                        Name = "Morning 1",
                        StartTime = new TimeSpan(6, 50, 0),
                        EndTime = new TimeSpan(9, 20, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Shift
                    {
                        Id = Guid.Parse("e7aa827f-cf74-4d2e-a022-8c45b9ecb274"),
                        Name = "Morning 2",
                        StartTime = new TimeSpan(9, 30, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Shift
                    {
                        Id = Guid.Parse("156a7644-bb5f-44e7-94e5-7aa5fc97e907"),
                        Name = "Afternoon 1",
                        StartTime = new TimeSpan(12, 50, 0),
                        EndTime = new TimeSpan(15, 20, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Shift
                    {
                        Id = Guid.Parse("1aebd4ec-da2f-4a09-a666-95cb54241ddb"),
                        Name = "Afternoon 2",
                        StartTime = new TimeSpan(15, 30, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Shift
                    {
                        Id = Guid.Parse("07b3b8a4-83e7-4a64-87a5-a32223b2dffd"),
                        Name = "Morning Full",
                        StartTime = new TimeSpan(6, 50, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Shift
                    {
                        Id = Guid.Parse("fc3c82b3-7ec1-4792-a290-2210667059ea"),
                        Name = "Afternoon Full",
                        StartTime = new TimeSpan(12, 50, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                await context.Shifts.AddRangeAsync(shifts);
                await context.SaveChangesAsync();
            }
        }
    }
}
