using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMS_UI.DTOs;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Controllers
{
    public class PolicyController : Controller
    {
        private readonly IRolePolicyService _rolePolicyService;
        private readonly IRolePolicyQueryService _rolePolicyQueryService;

        public PolicyController(IRolePolicyService rolePolicyService, IRolePolicyQueryService rolePolicyQueryService)
        {
            _rolePolicyService = rolePolicyService;
            _rolePolicyQueryService = rolePolicyQueryService;
        }

        [Authorize(Policy = "CanManagePolicies")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            var pagedPolicies = await _rolePolicyQueryService.GetPaginatedPoliciesAsync(pageIndex, pageSize);
            var roles = await _rolePolicyService.GetAllRolesAsync();

            var roleVMs = roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            ViewBag.Roles = roleVMs;

            return View(pagedPolicies);
        }

        [Authorize(Policy = "CanCreatePolicies")]
        [HttpPost]
        public async Task<IActionResult> CreatePolicy(string policyName, string description)
        {
            await _rolePolicyService.CreatePolicyAsync(new CreatePolicyDTO { Name = policyName, Description = description });
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CanManagePolicies")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var roles = await _rolePolicyService.GetAllRolesAsync();
            var allPolicies = await _rolePolicyService.GetAllPolicyAsync();

            var policy = allPolicies.FirstOrDefault(p => p.Id == id);
            if (policy == null) return NotFound();

            var assignedRoles = roles.Where(r =>
                r.RolePolicies.Any(rp => rp.PolicyId == id)).ToList();

            var viewModel = new RolePolicyAssignmentViewModel
            {
                RoleId = 0,
                RoleName = "",
                PolicyName = policy.Name,
                PolicyId = policy.Id,
                AllPolicies = allPolicies.Select(p => new PolicyViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList(),
                AssignedPolicyIds = assignedRoles.Select(r => r.Id).ToList()
            };

            ViewBag.PolicyId = id;
            ViewBag.PolicyName = policy.Name;
            ViewBag.AllRoles = roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Policy = "CanAssignPolicies")]
        [HttpPost]
        public async Task<IActionResult> AssignPolicy(int roleId, int policyId)
        {
            await _rolePolicyService.AssignPolicyToRoleAsync(new AssignPolicyToRoleDTO
            {
                RoleId = roleId,
                PolicyIds = new List<int> { policyId }
            });

            return RedirectToAction("Details", new { id = policyId });
        }

        [Authorize(Policy = "CanAssignPolicies")]
        [HttpPost]
        public async Task<IActionResult> RemovePolicyFromRole(int roleId, int policyId)
        {
            await _rolePolicyService.RemovePolicyFromRoleAsync(roleId, policyId);
            return RedirectToAction("Details", new { id = policyId });
        }

        [Authorize(Policy = "CanDeletePolicies")]
        [HttpPost]
        public async Task<IActionResult> RemovePolicy(int policyId)
        {
            await _rolePolicyService.RemovePolicyAsync(policyId);
            return RedirectToAction("Index");
        }
    }
}
