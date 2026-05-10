using SafeCity_DCRDB.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeCity.DCR.DTOs
{
    public class UpdateProgressRequest
    {
        [Required(ErrorMessage = "Response Status is required")]
        public ResponseStatus NewStatus { get; set; }
        [Required(ErrorMessage = "New Update Note is required")]
        public string UpdateNote { get; set; } = default!;
    }

    public class CloseMissionRequest
    {
        public string FinalClosingNote { get; set; } = default!;
    }
}
