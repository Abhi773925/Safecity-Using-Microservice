using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.Services
{
    public interface ICaseClosingService
    {
        public Task CaseClosing(CaseClosingRequest request, int CaseID);
    }
}
