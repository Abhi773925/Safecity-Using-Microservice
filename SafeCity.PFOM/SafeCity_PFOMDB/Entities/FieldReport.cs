using SafeCity_PFOMDB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_PFOMDB.Entities;

[Table("FieldReport")]
public class FieldReport
{
    [Key]
    public int ReportId { get; set; }

    [Required(ErrorMessage = "PatrolId can't be null")]
    public int PatrolId { get; set; }

    [Column(TypeName = "VARCHAR(100)")]
    public string Notes { get; set; } = default!;

    public DateTime Date { get; set; }

    [Column(TypeName = "VARCHAR(20)")]
    public FieldReportStatus Status { get; set; }

    [ForeignKey(nameof(PatrolId))]
    public virtual Patrol Patrol { get; set; } = default!;
}
