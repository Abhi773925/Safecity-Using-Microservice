using SafeCity.DCR.DTOs;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Services
{
    public interface ICrisisService
    {
        Task<CrisisResponse> CreateCrisis(CrisisRequest crisisRequest);

        Task<IEnumerable<CrisisResponse>> GetCrises(bool onlyActive);

        Task<bool> UpdateCrisisDetail(int id, CrisisStatus? status, CrisisSeverity? severity);

        Task<CrisisResponse?> GetCrisisDetails(int id);
    }
}