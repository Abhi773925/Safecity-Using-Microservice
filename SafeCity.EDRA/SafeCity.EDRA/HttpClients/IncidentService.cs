using SafeCity.EDRA.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SafeCity.EDRA.HttpClients
{
    public class IncidentService : IIncidentService
    {
        private readonly HttpClient _client;
        public IncidentService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<IncidentResponse>> GetIncidentsAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("api/incident/list/all");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(content))
            {
                return new List<IncidentResponse>();
            }

            var result = JsonSerializer.Deserialize<IncidentServiceWrapper>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Data ?? new List<IncidentResponse>();
        }

        public async Task UpdateIncidentStatusAsync(int incidentId, int option)
        {
            var response = await _client.PatchAsync($"api/incident/{incidentId}/status?option={option}", null);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception(string.IsNullOrWhiteSpace(content)
                    ? "Incident status update failed."
                    : content);
            }
        }
    }
}