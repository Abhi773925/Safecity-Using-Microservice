using SafeCity_IRCMDB.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_IRCMDB.Entity
{
    public class Case
    {
        [Key]
        public int CaseID { get; set; }

        [Required(ErrorMessage = "Incident Id is required")]
        public int IncidentID { get; set; }

        [Required(ErrorMessage = "Assigned Officer Id is Required")]
        public int AssignedOfficerID { get; set; }

        [Required(ErrorMessage = "Please fill some description to know more about the case")]
        public string Description { get; set; } = string.Empty;

        public CaseStatusCheck Status { get; set; } = CaseStatusCheck.Open;

        [Column(TypeName = "DATETIME")]
        public DateTime ResolutionDate { get; set; }

        [ForeignKey("IncidentID")]
        public virtual Incident? Incident { get; set; }
    }
}