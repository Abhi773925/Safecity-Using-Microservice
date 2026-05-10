using Microsoft.EntityFrameworkCore;
using SafeCity_DCRDB.Entities;

namespace SafeCity_DCRDB.Data
{
    public class SafeCityDbContext : DbContext
    {
        public SafeCityDbContext(DbContextOptions<SafeCityDbContext> options) : base(options)
        {
        }

        public DbSet<Crisis> Crises => Set<Crisis>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Response> Responses => Set<Response>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crisis>(entity =>
            {
                entity.HasKey(e => e.CrisisID);
                entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Severity).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.TeamID);
                entity.Property(e => e.TeamName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.ResponseID);
                entity.Property(e => e.Actions).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.HasOne(e => e.CrisisIdNavigation)
                      .WithMany(e => e.Responses)
                      .HasForeignKey(e => e.CrisisID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TeamIdNavigation)
                      .WithMany(e => e.Responses)
                      .HasForeignKey(e => e.TeamID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
