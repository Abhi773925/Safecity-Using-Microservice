using SafeCity.IAMDB.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeCity.IAMDB.Entities
{
    public class UserRole
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        public UserRoleOption RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
