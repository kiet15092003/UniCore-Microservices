using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Material> Materials { get; set; }  
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CourseCertificate> CourseCertificates { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<TrainingRoadmap> TrainingRoadmaps { get; set; }
        public DbSet<TrainingRoadmapCourse> TrainingRoadmapCourses { get; set; }
        public DbSet<CoursesGroup> CoursesGroups { get; set; }
        public DbSet<CoursesGroupSemester> CoursesGroupSemesters { get; set; }
        public DbSet<AcademicClass> AcademicClasses { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ScheduleInDay> ScheduleInDays { get; set; }        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Create a unique constraint for GroupName within each MajorId
            modelBuilder.Entity<CoursesGroup>()
                .HasIndex(g => new { g.GroupName })
                .IsUnique();
                
            modelBuilder.Entity<AcademicClass>()
                .HasMany(ac => ac.ScheduleInDays)
                .WithOne(sd => sd.AcademicClass)
                .HasForeignKey(sd => sd.AcademicClassId);

            // Configure parent-child relationship for AcademicClass
            modelBuilder.Entity<AcademicClass>()
                .HasOne(ac => ac.ParentTheoryAcademicClass)
                .WithMany(ac => ac.ChildPracticeAcademicClasses)
                .HasForeignKey(ac => ac.ParentTheoryAcademicClassId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid circular references

            base.OnModelCreating(modelBuilder);
        }

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
