using SafeCity_DCRDB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_DCRDB.Entities;

[Table("Crisis")]
public class Crisis
{
    [Key]
    public int CrisisID { get; set; }

    [Required]
    public CrisisType Type { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = default!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public CrisisSeverity Severity { get; set; } = CrisisSeverity.Low;

    [Required]
    public CrisisStatus Status { get; set; } = CrisisStatus.Pending;

    public ICollection<Response> Responses { get; set; } = new List<Response>();
}
