using Microsoft.EntityFrameworkCore;
using SafeCity_IRCMDB.Entity;

namespace SafeCity_IRCMDB.Data
{
    public class SafeCityDbContext : DbContext
    {
        public SafeCityDbContext(DbContextOptions<SafeCityDbContext> options) : base(options)
        {
        }

        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<Case> Cases => Set<Case>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LTIN718874\\SQLEXPRESS;Database=SafeCity_IRCMDB;Trusted_Connection=True;TrustServerCertificate=True");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Incident>(entity =>
            {
                entity.HasKey(e => e.IncidentID);
                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
                entity.Property(e => e.Location)
                    .HasColumnType("varchar(max)")
                    .IsRequired();
                entity.Property(e => e.Status)
                    .HasConversion<int>();
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(e => e.CaseID);
                entity.Property(e => e.Description).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.Status)
                    .HasConversion<int>();
                entity.Property(e => e.ResolutionDate)
                    .HasColumnType("DATETIME");
                entity.HasOne(e => e.Incident)
                    .WithMany()
                    .HasForeignKey(e => e.IncidentID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}