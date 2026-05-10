using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.Repositories;
using SafeCity_IRCMDB.Entity;

namespace SafeCity.IRCM.Services
{
    public class CaseCreateService : ICaseCreateService
    {
        // dependency injection logic
        private readonly ICaseCreateRepository _repository;
        private readonly IIncidentService _incidentService;
        public CaseCreateService(ICaseCreateRepository repository, IIncidentService incidentService)
        {
            _repository = repository;
            _incidentService = incidentService;
        }

        // Case Creation and validation logic goes here..
        public async Task<CaseCreateResponse> CaseCreate(CaseCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // checking all the field validations

            var errorList = new List<string>();
            if (request.IncidentID <= 0)
            {
                errorList.Add("IncidentID is missing ");
            }
            if (request.AssignedOfficerID <= 0)
            {
                errorList.Add("Assigned Officer Id is Missing");
            }
            if (request.Description == null)
            {
                errorList.Add("Description is missing");
            }
            if (request.Status < 0)
            {
                errorList.Add("Status is missing");
            }

            // check if any issue was found

            if (errorList.Count > 0)
            {
                throw new Exception(string.Join(" | ", errorList));
            }
            else
            {
                var response = await _repository.CaseCreate(request);
                // case create ho gya toh ab hume progess change krna hai incident jo create huya tha

                await _incidentService.IncidentStatusUpdate(request.IncidentID, 1);
                return response;
            }
        }

        public async Task<List<Case>> GetAllCase()
        {
            var response = await _repository.GetAllCase();
            return response;
        }

        public async Task<List<Case>> GetCaseByCitizenId(int citizenId)
        {
            var response = await _repository.GetCaseByCitizenId(citizenId);
            return response;
        }
    }
}
