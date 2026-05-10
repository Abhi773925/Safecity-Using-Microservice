using SafeCity.DCR.DTOs;
using SafeCity_DCRDB.Entities;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Repositories
{
    public interface ICrisisRepository
    {
        Task<CrisisResponse> CreateCrisis(CrisisRequest crisisRequest);

        Task<IEnumerable<CrisisResponse>> GetAllCrises();

        Task<IEnumerable<CrisisResponse>> GetActiveCrises();

        Task<bool> UpdateCrisis(int id, CrisisStatus? status, CrisisSeverity? severity);

        Task<Crisis?> GetCrisisById(int id);
    }
}