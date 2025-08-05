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
        public RoleController(IRolePolicyService rolePolicyService, IRolePolicyQueryService rolePolicyQueryService)
        {
            _rolePolicyService = rolePolicyService;
            _rolePolicyQueryService = rolePolicyQueryService;
        }

        [Authorize(Policy = "CanManageRoles")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            var pagedRoles = await _rolePolicyQueryService.GetPaginatedRolesAsync(pageIndex, pageSize);
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
            await _rolePolicyService.RemoveRoleAsync(roleId);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CanCreateRoles")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            await _rolePolicyService.CreateRoleAsync(new CreateRoleDTO { Name = roleName });
            return RedirectToAction("Index");
        }
    }
}
