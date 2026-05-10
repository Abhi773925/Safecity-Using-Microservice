using System.Text.Json;

namespace SafeCity.PFOM.HttpClients
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _client;

        public IdentityService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> IsOfficerValidAsync(int officerId)
        {
            var response = await _client.GetAsync($"api/user/internal/{officerId}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return false;
            }

            var data = JsonSerializer.Deserialize<InternalUserResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data != null
                && data.UserID > 0
                && string.Equals(data.RoleName, "Police", StringComparison.OrdinalIgnoreCase);
        }

        private class InternalUserResponse
        {
            public int UserID { get; set; }
            public string RoleName { get; set; } = default!;
        }
    }
}
