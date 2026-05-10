using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Repositories
{
    public interface IIncidentRepository
    {
        public Task<IncidentCreateResponse> IncidentCreate(IncidentCreateRequest request);
        public Task IncidentStatusUpdate(int IncidentID, int option);

    }
}
