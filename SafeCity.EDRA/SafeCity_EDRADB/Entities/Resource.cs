using SafeCity_EDRADB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_EDRADB.Entities
{
    [Table("Resource")]
    public class Resource
    {
        [Key]
        public int ResourceID { get; set; }

        [Required(ErrorMessage = "Resource type is required")]
        [Column(TypeName = "varchar(20)")]
        public ResourceTypeOption Type { get; set; }

        [Required(ErrorMessage = "Availability status is required")]
        [Column(TypeName = "varchar(20)")]
        public ResourceAvailabilityOption Availability { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Column(TypeName = "varchar(max)")]
        public string Location { get; set; } = default!;

        [Column(TypeName = "varchar(100)")]
        public string UnitName { get; set; } = default!;
    }
}
