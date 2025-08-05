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
        private readonly ILogger<PolicyController> _logger;

        public PolicyController(IRolePolicyService rolePolicyService, IRolePolicyQueryService rolePolicyQueryService, ILogger<PolicyController> logger)
        {
            _rolePolicyService = rolePolicyService;
            _rolePolicyQueryService = rolePolicyQueryService;
            _logger = logger;
        }

        [Authorize(Policy = "CanManagePolicies")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Getting Paginated Policies");
            var pagedPolicies = await _rolePolicyQueryService.GetPaginatedPoliciesAsync(pageIndex, pageSize);
            _logger.LogInformation("Getting Roles");
            var roles = await _rolePolicyService.GetAllRolesAsync();

            var roleVMs = roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            _logger.LogInformation("Roled bound to the view model");

            ViewBag.Roles = roleVMs;

            return View(pagedPolicies);
        }

        [Authorize(Policy = "CanCreatePolicies")]
        [HttpPost]
        public async Task<IActionResult> CreatePolicy(string policyName, string description)
        {
            _logger.LogInformation("Starting the creation of a new policy");
            await _rolePolicyService.CreatePolicyAsync(new CreatePolicyDTO { Name = policyName, Description = description });
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CanManagePolicies")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Getting list of Roles");
            var roles = await _rolePolicyService.GetAllRolesAsync();
            _logger.LogInformation("Gettign list of Policies");
            var allPolicies = await _rolePolicyService.GetAllPolicyAsync();

            var policy = allPolicies.FirstOrDefault(p => p.Id == id);
            if (policy == null)
            {
                _logger.LogInformation("Policy not found in the db");
                return NotFound();
            }

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
            _logger.LogInformation("Calling Service to assign Polcies to role");
            await _rolePolicyService.AssignPolicyToRoleAsync(new AssignPolicyToRoleDTO
            {
                RoleId = roleId,
                PolicyIds = new List<int> { policyId }
            });

            _logger.LogInformation("Policies Assigned Successfully");

            return RedirectToAction("Details", new { id = policyId });
        }

        [Authorize(Policy = "CanAssignPolicies")]
        [HttpPost]
        public async Task<IActionResult> RemovePolicyFromRole(int roleId, int policyId)
        {
            _logger.LogInformation("Calling Service to unassign policies from role");
            await _rolePolicyService.RemovePolicyFromRoleAsync(roleId, policyId);
            _logger.LogInformation("Polcies unassigned successfully");
            return RedirectToAction("Details", new { id = policyId });
        }

        [Authorize(Policy = "CanDeletePolicies")]
        [HttpPost]
        public async Task<IActionResult> RemovePolicy(int policyId)
        {
            _logger.LogInformation("Calling service to delete policy");
            await _rolePolicyService.RemovePolicyAsync(policyId);
            _logger.LogInformation("Policy deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
