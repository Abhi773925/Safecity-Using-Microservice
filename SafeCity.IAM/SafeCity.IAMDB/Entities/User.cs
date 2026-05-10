using Microsoft.EntityFrameworkCore;
using SafeCity.IAMDB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity.IAMDB.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Name { get; set; } = default!;

        [Required]
        public int RoleID { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(254)")]
        public string Email { get; set; } = default!;

        [Column(TypeName = "VARCHAR(20)")]
        public string Phone { get; set; } = default!;

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public UserStatus Status { get; set; } = UserStatus.Active;

        [ForeignKey("RoleID")]
        public virtual UserRole? UserRole { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string Password { get; set; } = default!;
    }
}
