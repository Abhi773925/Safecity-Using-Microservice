using Microsoft.EntityFrameworkCore;
using SafeCity_PFOMDB.Entities;

namespace SafeCity_PFOMDB.Data
{
    public class SafeCityDbContext : DbContext
    {
        public SafeCityDbContext(DbContextOptions<SafeCityDbContext> options) : base(options)
        {
        }

        public DbSet<Patrol> Patrols => Set<Patrol>();
        public DbSet<FieldReport> FieldReports => Set<FieldReport>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patrol>(entity =>
            {
                entity.HasKey(e => e.PatrolId);
                entity.Property(e => e.Area).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<FieldReport>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.Notes).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.HasOne(e => e.Patrol)
                      .WithMany(e => e.FieldReports)
                      .HasForeignKey(e => e.PatrolId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
