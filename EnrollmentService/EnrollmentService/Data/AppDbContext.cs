using EnrollmentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ScoreType> ScoreTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure ScoreType entity
            modelBuilder.Entity<ScoreType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Percentage).IsRequired();
            });
        }
    }
} 