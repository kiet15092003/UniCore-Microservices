using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Saves changes to the database when there is no HttpContext available (e.g., in background services)
        /// </summary>
        public static async Task<int> SaveChangesWithoutHttpContextAsync(this AppDbContext context, CancellationToken cancellationToken = default)
        {
            // Handle the BaseEntity properties without relying on HttpContext
            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // Use system account or specify a specific value for background operations
                    entry.Entity.CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"); // System account ID
                    entry.Entity.UpdatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000");
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000");
                }
            }

            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
