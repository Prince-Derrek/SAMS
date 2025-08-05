using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMS_UI.DTOs;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRolePolicyService _rolePolicyService;
        private readonly IRolePolicyQueryService _rolePolicyQueryService;
        private readonly ILogger<RoleController> _logger;
        public RoleController(IRolePolicyService rolePolicyService, IRolePolicyQueryService rolePolicyQueryService, ILogger<RoleController> logger)
        {
            _rolePolicyService = rolePolicyService;
            _rolePolicyQueryService = rolePolicyQueryService;
            _logger = logger;
        }

        [Authorize(Policy = "CanManageRoles")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Calling Service to Retrieve paginated policies");
            var pagedRoles = await _rolePolicyQueryService.GetPaginatedRolesAsync(pageIndex, pageSize);
            _logger.LogInformation("Calling Service to retrieve policies");
            var policies = await _rolePolicyService.GetAllPolicyAsync();

            var policyVMs = policies.Select(p => new PolicyViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).ToList();

            ViewBag.Policies = policyVMs;

            return View(pagedRoles);
        }

        [Authorize(Policy = "CanDeleteRoles")]
        [HttpPost]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            _logger.LogInformation("Callinf Service to delete roles");
            await _rolePolicyService.RemoveRoleAsync(roleId);
            _logger.LogInformation("Roles deleted successfully");
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CanCreateRoles")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            _logger.LogInformation("Calling service to create new role");
            await _rolePolicyService.CreateRoleAsync(new CreateRoleDTO { Name = roleName });
            _logger.LogInformation("Role created successfully");
            return RedirectToAction("Index");
        }
    }
}
