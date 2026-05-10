using System.ComponentModel.DataAnnotations;

namespace SafeCity.DCR.DTOs
{
    public class DeploymentRequest
    {
        [Required] public int CrisisId { get; set; }
        [Required] public int TeamId { get; set; }
        public string SpecialInstructions { get; set; } = "Proceed with caution.";
    }

    public class DeploymentResponse
    {
        public int ResponseId { get; set; }
        public int CrisisId { get; set; }
        public string CrisisLocation { get; set; } = default!;
        public string TeamName { get; set; } = default!;
        public int TeamLead { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime DeployedAt { get; set; }
        public string Instructions { get; set; } = default!;
    }
}
