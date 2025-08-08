using System.Security.Claims;

namespace SAMS_UI.Services.Interfaces
{
    public interface IViewPolicyService
    {
        Task<List<string>> GetPoliciesForCurrentUserAsync(ClaimsPrincipal user);
    }
}
