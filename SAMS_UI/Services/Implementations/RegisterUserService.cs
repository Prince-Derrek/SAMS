using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthTokenManager _tokenManager;
        private readonly ILogger<RegisterUserService> _logger;

        public RegisterUserService(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager, ILogger<RegisterUserService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _logger = logger;
        }
        public async Task<bool> CreateUserAsync(RegisterViewModel user)
        {
            _logger.LogInformation("Calling token manager service");
            var token = await _tokenManager.GetTokenAsync();

            _logger.LogInformation("Creating client");
            var client = _httpClientFactory.CreateClient("BackendApi");
            _logger.LogInformation("Client created successfully");

            _logger.LogInformation("Binding token to header");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogInformation("Bind successful");

            _logger.LogInformation("Serializing new user data");
            var json = JsonSerializer.Serialize(user);
            _logger.LogInformation("User data serialized successfully");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Attempting to reach register endpoint");
            var response = await client.PostAsync("/api/user/register", content);
            _logger.LogInformation("Endpoint reached successfully");

            return response.IsSuccessStatusCode;
        }
    }
}
