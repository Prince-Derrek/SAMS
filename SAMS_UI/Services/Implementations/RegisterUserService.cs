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

        public RegisterUserService(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
        }
        public async Task<bool> CreateUserAsync(RegisterViewModel user)
        {
            var token = await _tokenManager.GetTokenAsync();
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/user/register", content);
            return response.IsSuccessStatusCode;
        }
    }
}
