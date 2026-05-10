using SafeCity.IRCM.DTOs;
using SafeCity_IRCMDB.Entity;

namespace SafeCity.IRCM.Repositories
{
    public interface ICaseCreateRepository
    {
        public Task<CaseCreateResponse> CaseCreate(CaseCreateRequest request);
        public Task<List<Case>> GetAllCase();
        public Task<List<Case>> GetCaseByCitizenId(int citizenId);
    }
}
