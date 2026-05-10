using SafeCity_EDRADB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_EDRADB.Entities
{
    [Table("Dispatch")]
    public class Dispatch
    {
        [Key]
        public int DispatchID { get; set; }

        [Required(ErrorMessage = "Incident id is required")]
        public int IncidentID { get; set; }

        [Required(ErrorMessage = "Dispatcher id is required")]
        public int DispatcherID { get; set; }

        public int? ResourceID { get; set; }

        [Required(ErrorMessage = "Dispatch status is required")]
        [Column(TypeName = "varchar(20)")]
        public DispatchStatusOption Status { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
