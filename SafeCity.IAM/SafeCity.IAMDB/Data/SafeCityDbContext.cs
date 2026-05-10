using Microsoft.EntityFrameworkCore;
using SafeCity.IAMDB.Entities;
using SafeCity.IAMDB.Enums;

namespace SafeCity.IAMDB.Data
{
    public class SafeCityDbContext : DbContext
    {
        public SafeCityDbContext(DbContextOptions<SafeCityDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(temp => temp.RoleID);
                entity.Property(temp => temp.RoleName).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(temp => temp.UserID);
                entity.Property(temp => temp.Name).IsRequired().HasMaxLength(100);
                entity.Property(temp => temp.Email).IsRequired().HasMaxLength(254);
                entity.Property(temp => temp.Phone).HasMaxLength(20);
                entity.Property(temp => temp.Password).IsRequired().HasMaxLength(500);
                entity.HasIndex(temp => temp.Email).IsUnique();
                entity.HasOne(temp => temp.UserRole)
                      .WithMany(temp => temp.Users)
                      .HasForeignKey(temp => temp.RoleID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { RoleID = 1, RoleName = UserRoleOption.Citizen },
                new UserRole { RoleID = 2, RoleName = UserRoleOption.Police },
                new UserRole { RoleID = 3, RoleName = UserRoleOption.Fire_Fighter },
                new UserRole { RoleID = 4, RoleName = UserRoleOption.Emergency_Dispatcher },
                new UserRole { RoleID = 5, RoleName = UserRoleOption.Compliance_Officer },
                new UserRole { RoleID = 6, RoleName = UserRoleOption.City_Administrator }
            );
        }
    }
}
