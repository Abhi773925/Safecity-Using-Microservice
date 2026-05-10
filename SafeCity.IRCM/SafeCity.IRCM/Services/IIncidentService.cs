using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Services
{
    public interface IIncidentService
    {
        public Task<IncidentCreateResponse> IncidentCreate(IncidentCreateRequest request);
        public Task IncidentStatusUpdate(int IncidentID, int option);

    }
}
