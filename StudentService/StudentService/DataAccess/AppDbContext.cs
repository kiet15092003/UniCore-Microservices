using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudentService.Entities;

namespace StudentService.DataAccess
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Guardian> Guardians { get; set; }

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guardian>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
               .HasOne(p => p.Guardian)
               .WithMany()
               .HasForeignKey(p => p.GuardianId)
               .IsRequired(false);
        }
    }
}
