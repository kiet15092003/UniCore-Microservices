using EnrollmentService.Entities;
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
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentResult> StudentResults { get; set; }
        public DbSet<AttendanceInDay> AttendanceInDays { get; set; }
        public DbSet<ScoreType> ScoreTypes { get; set; }
        public DbSet<EnrollmentExam> EnrollmentExams { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
