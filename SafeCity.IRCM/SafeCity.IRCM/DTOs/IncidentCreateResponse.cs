using SafeCity_IRCMDB.Entity;

namespace SafeCity.IRCM.DTOs
{
    public class IncidentCreateResponse
    {
        public int IncidentID { get; set; }
        public int CitizenID { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }

    public static class IncidentCreateResponseExtension
    {
        public static IncidentCreateResponse ToIncidentCreateResponse(Incident incident)
        {
            return new IncidentCreateResponse()
            {
                IncidentID = incident.IncidentID,
                CitizenID = incident.CitizenID,
                Type = incident.Type.ToString(),
                Location = incident.Location,
                Date = incident.Date,
                Status = incident.Status.ToString(),

            };
        }
    }
}
