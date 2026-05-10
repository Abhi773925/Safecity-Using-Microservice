using SafeCity_DCRDB.Entities;
using SafeCity_DCRDB.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeCity.DCR.DTOs
{
    public class CrisisRequest
    {
        [Required]
        public CrisisType Type { get; set; }
        [Required]
        public string Location { get; set; } = default!;
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required]
        public CrisisSeverity Severity { get; set; } = CrisisSeverity.Low;
        [Required]
        public CrisisStatus Status { get; set; } = CrisisStatus.Pending;

        public Crisis ToCrisisRequest()
        {
            return new Crisis
            {
                Type = Type,
                Location = Location,
                Date = Date,
                Severity = Severity,
                Status = Status
            };
        }
    }

    public class CrisisResponse
    {
        public int CrisisId { get; set; }
        [Required]
        public string Type { get; set; } = default!;
        [Required]
        public string Location { get; set; } = default!;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Severity { get; set; } = default!;
        [Required]
        public string Status { get; set; } = default!;
    }

    public static class CrisisResponseExtension
    {
        public static CrisisResponse ToCrisisResponse(Crisis crisis)
        {
            return new CrisisResponse
            {
                CrisisId = crisis.CrisisID,
                Type = crisis.Type.ToString(),
                Location = crisis.Location,
                Date = crisis.Date,
                Severity = crisis.Severity.ToString(),
                Status = crisis.Status.ToString()
            };
        }
    }
}
