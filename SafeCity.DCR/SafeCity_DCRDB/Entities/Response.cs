using SafeCity_DCRDB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCity_DCRDB.Entities;

[Table("Response")]
public class Response
{
    [Key]
    public int ResponseID { get; set; }

    [Required]
    public int CrisisID { get; set; }

    [Required]
    public int TeamID { get; set; }

    [Required, MaxLength(1000)]
    public string Actions { get; set; } = default!;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public ResponseStatus Status { get; set; } = ResponseStatus.Pending;

    [ForeignKey(nameof(CrisisID))]
    public virtual Crisis CrisisIdNavigation { get; set; } = default!;

    [ForeignKey(nameof(TeamID))]
    public virtual Team TeamIdNavigation { get; set; } = default!;
}
