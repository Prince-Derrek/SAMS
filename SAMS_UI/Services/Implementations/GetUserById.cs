using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class GetUserById : IGetUserById
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthTokenManager _tokenManager;
        private readonly ILogger<GetUserById> _logger;

        public GetUserById(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager, ILogger<GetUserById> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _logger = logger;
        }
        public async Task<UserViewModel> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Calling token Manager service ");
            var token = await _tokenManager.GetTokenAsync();

            _logger.LogInformation("Creating client");
            var client = _httpClientFactory.CreateClient("BackendApi");
            _logger.LogInformation("Client created successfully");

            _logger.LogInformation("Binding token to headers");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogInformation("Token bound successfully");

            _logger.LogInformation("Attempting to reach get user by Id endpoint");
            var response = await client.GetAsync($"/api/user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed to reach get user by id endpoint");
                return new();
            }
            else
            {
                _logger.LogInformation("Get user by id endpoint reached successfully");
            }

            var json = await response.Content.ReadAsStreamAsync();
            var users = await JsonSerializer.DeserializeAsync<UserViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _logger.LogInformation("User returned");

            return users ?? new();
        }
    }
}
