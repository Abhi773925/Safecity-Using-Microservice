using Microsoft.EntityFrameworkCore;
using SafeCity_EDRADB.Entities;

namespace SafeCity_EDRADB.Data
{
    public class SafeCityDbContext : DbContext
    {
        public SafeCityDbContext(DbContextOptions<SafeCityDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources => Set<Resource>();
        public DbSet<Dispatch> Dispatches => Set<Dispatch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceID);
                entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Availability).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.UnitName).HasMaxLength(100);
            });

            modelBuilder.Entity<Dispatch>(entity =>
            {
                entity.HasKey(e => e.DispatchID);
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Date).IsRequired();
            });
        }
    }
}
