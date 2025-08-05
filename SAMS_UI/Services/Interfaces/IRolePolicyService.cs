using SAMS_UI.DTOs;
using SAMS_UI.Models;

namespace SAMS_UI.Services.Interfaces
{
    public interface IRolePolicyService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<List<Policy>> GetAllPolicyAsync();
        Task CreateRoleAsync(CreateRoleDTO dto);
        Task CreatePolicyAsync(CreatePolicyDTO dto);

        Task AssignPolicyToRoleAsync(AssignPolicyToRoleDTO dto);
        Task RemovePolicyFromRoleAsync(int roleId, int policyId);
        Task RemovePolicyAsync(int policyId);
        Task RemoveRoleAsync(int roleId);
    }
}
