using SAMS_UI.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class UpdateUserActivityStatus : IUpdateUserActivityStatus
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthTokenManager _tokenManager;

        public UpdateUserActivityStatus(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
        }
        public async Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive)
        {
            var token = await _tokenManager.GetTokenAsync();
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new { userId, isActive };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/user/activity-status")
            {
                Content = content
            };

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
