using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.Repositories;

namespace SafeCity.IRCM.Services
{
    public class IncidentRetrivalService : IIncidentRetrivalService
    {
        // dependency injection
        private readonly IIncidentRetrivalRepository _repository;
        public IncidentRetrivalService(IIncidentRetrivalRepository repository)
        {
            _repository = repository;
        }

        // fetch the incident with the pending status
        public async Task<List<IncidentCreateResponse>> IncidentRetrival()
        {
            try
            {
                var response = await _repository.IncidentRetrival();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("No Incident With Pending State Available");
            }
        }

        public async Task<List<IncidentCreateResponse>> IncidentRetrivalAll()
        {
            try
            {
                var response = await _repository.IncidentRetrivalAll();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("No Incident Available");
            }
        }

        public async Task<List<IncidentCreateResponse>> IncidentRetrivalByCitizenId(int citizenId)
        {
            try
            {
                var response = await _repository.IncidentRetrivalByCitizenId(citizenId);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("No Incident Available For This Citizen");
            }
        }
    }
}
