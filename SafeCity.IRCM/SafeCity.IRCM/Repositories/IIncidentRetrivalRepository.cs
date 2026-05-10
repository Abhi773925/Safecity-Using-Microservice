using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Repositories
{
    public interface IIncidentRetrivalRepository
    {
        public Task<List<IncidentCreateResponse>> IncidentRetrival();
        public Task<List<IncidentCreateResponse>> IncidentRetrivalAll();
        public Task<List<IncidentCreateResponse>> IncidentRetrivalByCitizenId(int citizenId);
    }
}
