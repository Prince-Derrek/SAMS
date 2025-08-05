using SAMS_UI.ViewModels;

namespace SAMS_UI.Services.Interfaces
{
    public interface IRolePolicyQueryService
    {
        Task<PaginatedListViewModel<RoleViewModel>> GetPaginatedRolesAsync(int pageIndex, int pageSize);
        Task<PaginatedListViewModel<PolicyViewModel>> GetPaginatedPoliciesAsync(int pageIndex, int pageSize);
    }
}
