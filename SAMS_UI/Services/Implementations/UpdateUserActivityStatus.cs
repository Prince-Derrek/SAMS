using SAMS_UI.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _db;

        public UpdateUserActivityStatus(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager, AppDbContext db)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _db = db;
        }

        public async Task<bool> UpdateBackendUserActivityStatusAsync(Guid userId, bool isActive)
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

        public async Task<bool> UpdateFrontendUserActivityStatusAsync(Guid userId, bool isActive)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            user.IsActive = isActive;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
