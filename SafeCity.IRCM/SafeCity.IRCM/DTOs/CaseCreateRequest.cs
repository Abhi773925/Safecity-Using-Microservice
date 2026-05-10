using SafeCity_IRCMDB.Entity;
using SafeCity_IRCMDB.Enum;

namespace SafeCity.IRCM.DTOs
{
    public class CaseCreateRequest
    {
        public int IncidentID { get; set; } = default!;
        public int AssignedOfficerID { get; set; } = default!;
        public string Description { get; set; } = default!;
        public CaseStatusCheck Status { get; set; } = CaseStatusCheck.Open;
        public DateTime ResolutionDate { get; set; } = default!;

        public Case ToCaseCreateRequest()
        {
            return new Case
            {
                IncidentID = IncidentID,
                AssignedOfficerID = AssignedOfficerID,
                Description = Description,
                Status = Status,
                ResolutionDate = ResolutionDate,
            };
        }
    }
}
