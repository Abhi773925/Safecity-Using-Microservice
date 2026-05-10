using SafeCity_IRCMDB.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_IRCMDB.Entity
{
    public class Incident
    {
        [Key]
        public int IncidentID { get; set; }

        [Required(ErrorMessage = "Citizen id is required")]
        public int CitizenID { get; set; }

        [Required(ErrorMessage = "Incident type is required")]
        [Column(TypeName = "varchar(20)")]
        public IncidentOption Type { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Column(TypeName = "varchar(max)")]
        public string Location { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public IncidentStatusOption Status { get; set; } = IncidentStatusOption.Pending;
    }
}