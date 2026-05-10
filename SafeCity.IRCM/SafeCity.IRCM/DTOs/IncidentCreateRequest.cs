using SafeCity_IRCMDB.Entity;
using SafeCity_IRCMDB.Enum;

namespace SafeCity.IRCM.DTOs
{
    public class IncidentCreateRequest
    {
        public IncidentOption Type { get; set; } = default!;
        public string Location { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public IncidentStatusOption Status { get; set; } = IncidentStatusOption.Pending;
        public int CitizenID { get; set; }

        public Incident ToIncidentCreateRequest()
        {
            return new Incident()
            {
                Type = Type,
                Location = Location,
                Date = Date,
                Status = Status,
                CitizenID = CitizenID
            };
        }
    }
}
