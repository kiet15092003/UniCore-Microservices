using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserService.Entities;

namespace UserService.DataAccess
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }        // Users is already defined in IdentityDbContext, so we don't redefine it here
        public DbSet<TrainingManager> TrainingManagers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Address> Addresses { get; set; }   

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

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(p => p.PersonId)
                .IsUnique();
                
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique();
            
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .IsRequired(); // vì bạn đã [Required] trong entity

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Student)
                .WithOne(s => s.ApplicationUser)
                .HasForeignKey<Student>(s => s.ApplicationUserId);
        }
    }
}
