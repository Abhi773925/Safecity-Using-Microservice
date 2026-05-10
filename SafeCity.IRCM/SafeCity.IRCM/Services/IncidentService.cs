using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.Repositories;

namespace SafeCity.IRCM.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _repository;
        public IncidentService(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IncidentCreateResponse> IncidentCreate(IncidentCreateRequest request)
        {
            //check if the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var errorList = new List<string>();
            if (request.Type == null)
            {
                errorList.Add("Type is missing");
            }
            if (request.Location == null)
            {
                errorList.Add("Location is missing");
            }
            if (request.Date == null)
            {
                errorList.Add("Date is missing");
            }
            if (request.Status == null)
            {
                errorList.Add("Status is missing");
            }
            //checking the valiation and throws error if any
            if (errorList.Count() > 0)
            {
                throw new Exception(string.Join(" | ", errorList));
            }

            //if all the validation paases
            //userId jo mujhe IAM Se milega
            var response = await _repository.IncidentCreate(request);

            return response;

        }

        // Update Incident Status Goes Hers
        public async Task IncidentStatusUpdate(int IncidentID, int option)
        {
            await _repository.IncidentStatusUpdate(IncidentID, option);
        }
    }
}
