using SafeCity.DCR.DTOs;
using SafeCity.DCR.Repositories;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Services
{
    public class CrisisService : ICrisisService
    {
        private readonly ICrisisRepository _crisisRepository;
        public CrisisService(ICrisisRepository crisisRepository)
        {
            _crisisRepository = crisisRepository;
        }

        public async Task<CrisisResponse> CreateCrisis(CrisisRequest crisisRequest)
        {
            // Yahan humne Repository ka logic call kiya jisme duplicate check hai
            return await _crisisRepository.CreateCrisis(crisisRequest);
        }

        public async Task<IEnumerable<CrisisResponse>> GetCrises(bool onlyActive)
        {
            return onlyActive
                ? await _crisisRepository.GetActiveCrises()
                : await _crisisRepository.GetAllCrises();
        }

        public async Task<bool> UpdateCrisisDetail(int id, CrisisStatus? status, CrisisSeverity? severity)
        {
            return await _crisisRepository.UpdateCrisis(id, status, severity);
        }

        public async Task<CrisisResponse?> GetCrisisDetails(int id)
        {
            var crisis = await _crisisRepository.GetCrisisById(id);
            if (crisis == null) return null;

            return CrisisResponseExtension.ToCrisisResponse(crisis);
        }
    }
}