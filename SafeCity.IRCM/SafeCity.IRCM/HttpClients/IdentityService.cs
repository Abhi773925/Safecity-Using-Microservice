using SafeCity.IRCM.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SafeCity.IRCM.HttpClients
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _client;

        public IdentityService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserResponseToken> GetLoggedInUsers(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    throw new Exception("Invalid JWT Token format.");
                }

                var jwtToken = handler.ReadJwtToken(token);

                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;


                var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                var result = new UserResponseToken
                {
                    Token = token,
                    Name = name ?? "Unknown",
                    Role = role ?? "Citizen",
                    UserID = int.TryParse(userIdStr, out int id) ? id : 0
                };

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Decoding Error: {ex.Message}");
            }
        }
    }
}