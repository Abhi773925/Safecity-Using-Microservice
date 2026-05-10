using SafeCity.IRCM.DTOs;

namespace SafeCity.IRCM.HttpClients
{
    public interface IIdentityService
    {
        public Task<UserResponseToken> GetLoggedInUsers(string Token);
    }
}
