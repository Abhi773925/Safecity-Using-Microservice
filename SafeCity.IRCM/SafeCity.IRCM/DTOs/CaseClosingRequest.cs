using SafeCity_IRCMDB.Entity;
using SafeCity_IRCMDB.Enum;

namespace SafeCity.IRCM.DTOs
{
    public class CaseClosingRequest
    {
        public DateTime ResolutionDate { get; set; } = default!;
        public string Description { get; set; } = default!;
        public CaseStatusCheck Status { get; set; } = CaseStatusCheck.Closed;

        public Case ToCaseClosingRequest()
        {
            return new Case()
            {
                ResolutionDate = DateTime.Now,
                Description = Description,
                Status = Status
            };
        }
    }
}
