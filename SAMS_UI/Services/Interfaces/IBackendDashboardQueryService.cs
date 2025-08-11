using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Interfaces
{
    public interface IBackendDashboardQueryService
    {
        Task<PaginatedListViewModel<BackendDashboardViewModel>> GetPaginatedUsersAsync(int pageIndex = 1, int pageSize = 10);
        Task<BackendDashboardViewModel> GetUserByIdAsync(Guid id);
        Task<bool> CreateUserAsync(RegisterViewModel user);
        Task<bool> UpdateUserActivityStatusAsync(Guid userId, bool isActive);
    }
}
