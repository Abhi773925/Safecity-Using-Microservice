using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Repositories
{
    public interface ICaseClosingRepository
    {
        public Task CaseClosing(CaseClosingRequest request, int CaseID);
    }
}
