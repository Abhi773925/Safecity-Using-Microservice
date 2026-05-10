using SafeCity_PFOMDB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_PFOMDB.Entities;

[Table("Patrol")]
public class Patrol
{
    [Key]
    public int PatrolId { get; set; }

    [Required(ErrorMessage = "OfficerId can't be null")]
    public int OfficerId { get; set; }

    [Column(TypeName = "VARCHAR(100)")]
    public string Area { get; set; } = default!;

    public DateTime Date { get; set; }

    [Column(TypeName = "VARCHAR(20)")]
    public PatrolStatus Status { get; set; }

    public virtual ICollection<FieldReport> FieldReports { get; set; } = new List<FieldReport>();
}
