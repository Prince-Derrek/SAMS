using System.Text;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class AuthTokenManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string? _token;

        public AuthTokenManager(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token)) return _token;

            var payload = new
            {
                id = _configuration["ServiceAuth:Id"],
                userSecret = _configuration["ServiceAuth:UserSecret"]
            };

            var client = _httpClientFactory.CreateClient("BackendApi");
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            _token = doc.RootElement.GetProperty("token").GetString();
            return _token;
        }

        public async Task<string?> GetMessageTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token)) return _token;

            var payload = new
            {
                userId = _configuration["ServiceAuth:ClientId"],
                userSecret = _configuration["ServiceAuth:ClientSecret"]
            };

            var client = _httpClientFactory.CreateClient("BackendApi");
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/auth/login", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            _token = doc.RootElement.GetProperty("token").GetString();
            return _token;
        }

        public void ClearToken() => _token = null;
    }
}
