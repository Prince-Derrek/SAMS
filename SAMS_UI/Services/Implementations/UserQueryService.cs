using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SAMS_UI.Services.Implementations
{
    public class UserQueryService : IUserQueryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthTokenManager _tokenManager;

        public async Task<PaginatedListViewModel<UserViewModel>> GetPaginatedUsersAsync(int pageIndex = 1, int pageSize = 10)
        {
            var token = await _tokenManager.GetTokenAsync();
            var client = _httpClientFactory.CreateClient("BackendApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUrl = $"/api/user?pageIndex={pageIndex}&pageSize={pageSize}";
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                return EmptyPaginatedResult(pageSize);
            }

            try
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var pagedUsers = await JsonSerializer.DeserializeAsync<PaginatedListViewModel<UserViewModel>>(stream, options);

                if (pagedUsers != null)
                {
                    pagedUsers.PageSize = pageSize;
                }

                return pagedUsers ?? EmptyPaginatedResult(pageSize);
            }
            catch (JsonException ex)
            {
                return EmptyPaginatedResult(pageSize);
            }
        }

        private static PaginatedListViewModel<UserViewModel> EmptyPaginatedResult(int pageSize)
        {
            return new PaginatedListViewModel<UserViewModel>
            {
                Items = new List<UserViewModel>(),
                PageIndex = 1,
                PageSize = pageSize,
                TotalCount = 0
            };
        }
    }
}
