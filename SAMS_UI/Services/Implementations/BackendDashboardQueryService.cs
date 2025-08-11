using SAMS_UI.DTOs;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class BackendDashboardQueryService : IBackendDashboardQueryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthTokenManager _tokenManager;
        private readonly ILogger<BackendDashboardQueryService> _logger;

        public BackendDashboardQueryService(IHttpClientFactory httpClientFactory, AuthTokenManager tokenManager, ILogger<BackendDashboardQueryService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        public async Task<PaginatedListViewModel<BackendDashboardViewModel>> GetPaginatedUsersAsync(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching paginated users. PageIndex: {PageIndex}, PageSize: {PageSize}", pageIndex, pageSize);

            var token = await _tokenManager.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No service token available. Aborting paginated users request.");
                return EmptyUserResult(pageIndex, pageSize);
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"/api/user?pageIndex={pageIndex}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to retrieve users. StatusCode: {StatusCode}", response.StatusCode);
                    return EmptyUserResult(pageIndex, pageSize);
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var pagedDto = await JsonSerializer.DeserializeAsync<PaginatedListViewModel<UserDTO>>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (pagedDto == null)
                {
                    _logger.LogWarning("Deserialized user DTO is null.");
                    return EmptyUserResult(pageIndex, pageSize);
                }

                // Map DTO → ViewModel
                var viewModels = pagedDto.Items.Select(u => new BackendDashboardViewModel
                {
                    Id = u.Id,
                    UserName = u.userName,
                    UserSecret = u.userSecret,
                    CreatedAt = u.CreatedAt,
                    Description = u.Description,
                    isActive = u.isActive
                }).ToList();

                _logger.LogInformation("Retrieved {Count} users.", viewModels.Count);

                return new PaginatedListViewModel<BackendDashboardViewModel>
                {
                    Items = viewModels,
                    TotalCount = pagedDto.TotalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = pagedDto.TotalPages
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error while fetching paginated users.");
                return EmptyUserResult(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching paginated users.");
                return EmptyUserResult(pageIndex, pageSize);
            }
        }

        private PaginatedListViewModel<BackendDashboardViewModel> EmptyUserResult(int pageIndex, int pageSize)
        {
            return new PaginatedListViewModel<BackendDashboardViewModel>
            {
                Items = new List<BackendDashboardViewModel>(),
                TotalCount = 0,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = 0,
                //HasPreviousPage = false,
                //HasNextPage = false
            };
        }

        public async Task<BackendDashboardViewModel> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);

            var token = await _tokenManager.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No service token available. Aborting user fetch by ID.");
                return new();
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"/api/user/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to retrieve user {UserId}. StatusCode: {StatusCode}", id, response.StatusCode);
                    return new();
                }

                var json = await response.Content.ReadAsStreamAsync();
                var user = await JsonSerializer.DeserializeAsync<BackendDashboardViewModel>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Successfully retrieved user: {UserId}", id);
                return user ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user by ID: {UserId}", id);
                return new();
            }
        }
        public async Task<bool> CreateUserAsync(RegisterViewModel user)
        {
            _logger.LogInformation("Creating a new user: {UserName}", user.userName);

            var token = await _tokenManager.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No service token available. Aborting user creation.");
                return false;
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/user/register", content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserName} created successfully", user.userName);
                    return true;
                }

                _logger.LogWarning("Failed to create user {UserName}. StatusCode: {StatusCode}", user.userName, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user {UserName}", user.userName);
                return false;
            }
        }

        public async Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive)
        {
            _logger.LogInformation("Updating activity status for user {UserId} to {Status}", userId, isActive);

            var token = await _tokenManager.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No service token available. Aborting update of user activity status.");
                return false;
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var payload = new { userId, isActive };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/user/activity-status")
                {
                    Content = content
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User {UserId} activity status updated to {Status}", userId, isActive);
                    return true;
                }

                _logger.LogWarning("Failed to update activity status for user {UserId}. StatusCode: {StatusCode}", userId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating activity status for user {UserId}", userId);
                return false;
            }
        }
    }
}
