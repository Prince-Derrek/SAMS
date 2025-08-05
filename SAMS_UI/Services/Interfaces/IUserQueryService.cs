using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Interfaces
{
    public interface IUserQueryService
    {
        Task<PaginatedListViewModel<UserViewModel>> GetPaginatedUsersAsync(int pageIndex = 1, int pageSize = 10);
    }
}
