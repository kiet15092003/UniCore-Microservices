using EnrollmentService.Data;
using EnrollmentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.DataAccess
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();
            
            // Seed score types if they don't exist
            await SeedScoreTypesAsync(context);
        }
        
        private static async Task SeedScoreTypesAsync(AppDbContext context)
        {
            if (!await context.ScoreTypes.AnyAsync())
            {
                var scoreTypes = new List<ScoreType>
                {
                    new ScoreType
                    {
                        Id = Guid.Parse("dba670c2-255d-40b5-afa5-135466d294fd"),
                        Type = 1, // Midterm
                        Percentage = 10,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ScoreType
                    {
                        Id = Guid.Parse("e7aa827f-cf74-4d2e-a022-8c45b9ecb274"),
                        Type = 2, // Final
                        Percentage = 20,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ScoreType
                    {
                        Id = Guid.Parse("156a7644-bb5f-44e7-94e5-7aa5fc97e907"),
                        Type = 3, // Assignment
                        Percentage = 20,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ScoreType
                    {
                        Id = Guid.Parse("1aebd4ec-da2f-4a09-a666-95cb54241ddb"),
                        Type = 4, // Quiz
                        Percentage = 50,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                await context.ScoreTypes.AddRangeAsync(scoreTypes);
                await context.SaveChangesAsync();
            }
        }
    }
} 