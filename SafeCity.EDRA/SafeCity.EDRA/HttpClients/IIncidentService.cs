using SafeCity.EDRA.DTOs;

namespace SafeCity.EDRA.HttpClients
{
    public interface IIncidentService
    {
        public Task<List<IncidentResponse>> GetIncidentsAsync(string token);
        public Task UpdateIncidentStatusAsync(int incidentId, int option);

    }
}
