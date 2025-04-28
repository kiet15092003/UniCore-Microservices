using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess
{
    public class AppDbContext : DbContext
    {private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Material> Materials { get; set; }  
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CourseCertificate> CourseCertificates { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<TrainingRoadmap> TrainingRoadmaps { get; set; }
        public DbSet<TrainingRoadmapCourse> TrainingRoadmapCourses { get; set; }
        public DbSet<CoursesGroup> CoursesGroups { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            Guid? userId = null;

            if (Guid.TryParse(userIdString, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
