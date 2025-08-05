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

        public GetUserById(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
        }
        public async Task<UserViewModel> GetUserByIdAsync(Guid id)
        {
            var token = await _tokenManager.GetTokenAsync();
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/user/{id}");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStreamAsync();
            var users = await JsonSerializer.DeserializeAsync<UserViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return users ?? new();
        }
    }
}
