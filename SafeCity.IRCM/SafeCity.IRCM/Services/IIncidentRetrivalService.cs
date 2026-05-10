using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Services
{
    public interface IIncidentRetrivalService
    {
        public Task<List<IncidentCreateResponse>> IncidentRetrival();
        public Task<List<IncidentCreateResponse>> IncidentRetrivalAll();
        public Task<List<IncidentCreateResponse>> IncidentRetrivalByCitizenId(int citizenId);
    }
}
