using System.Text;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class AuthTokenManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string? _token;
        private readonly ILogger<AuthTokenManager> _logger;

        public AuthTokenManager(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthTokenManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                _logger.LogInformation("Token initialized");
                return _token;
            }
            else
            {
                _logger.LogInformation("Failed to initialize token");
            }

                var payload = new
                {
                    id = _configuration["ServiceAuth:Id"],
                    userSecret = _configuration["ServiceAuth:UserSecret"]
                };
            _logger.LogInformation("Payload created successfully");

            _logger.LogInformation("Creating client");
            var client = _httpClientFactory.CreateClient("BackendApi");
            _logger.LogInformation("Client created successfully");

            _logger.LogInformation("Serializing Payload");
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _logger.LogInformation("Payload serialized successfully");

            _logger.LogInformation("Attempting to reach backend endpoint");
            var response = await client.PostAsync("/api/auth/login", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed to reach endpoint");
                return null;
            }
            else
            {
                _logger.LogInformation("Successfully reached endpoint");
            }

            _logger.LogInformation("Successfully received token");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            _token = doc.RootElement.GetProperty("token").GetString();
            return _token;
        }
        public void ClearToken() => _token = null;
    }
}
