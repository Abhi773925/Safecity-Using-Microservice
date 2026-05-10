using SafeCity_IRCMDB.Entity;
using SafeCity_IRCMDB.Enum;

namespace SafeCity.IRCM.DTOs
{
    public class CaseCreateResponse
    {
        public int CaseID { get; set; }
        public int IncidentID { get; set; }
        public int AssignedOfficerID { get; set; }
        public string Description { get; set; }
        public CaseStatusCheck Status { get; set; }
        public DateTime ResolutionDate { get; set; }
    }

    public static class CaseCreateResponseExtension
    {
        public static CaseCreateResponse ToCaseCreateResponse(Case cases)
        {
            return new CaseCreateResponse
            {
                CaseID = cases.CaseID,
                IncidentID = cases.IncidentID,
                AssignedOfficerID = cases.AssignedOfficerID,
                Description = cases.Description,
                Status = cases.Status,
                ResolutionDate = cases.ResolutionDate
            };
        }
    }
}
