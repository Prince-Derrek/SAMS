using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Security.Claims;

namespace SAMS_UI.Controllers
{
    public class BackendController : Controller
    {
        private readonly IBackendDashboardQueryService _dashboardQuery;
        private readonly ILogger<BackendController> _logger;

        public BackendController(IBackendDashboardQueryService dashboardQuery, ILogger<BackendController> logger)
        {
            _dashboardQuery = dashboardQuery;
            _logger = logger;
        }

        [Authorize(Policy = "CanViewDashboards")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation(
            "User {UserName} requested paginated dashboards list. PageIndex: {PageIndex}, PageSize: {PageSize}",
            User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown", pageIndex, pageSize);

            var pagedDashboards = await _dashboardQuery.GetPaginatedUsersAsync(pageIndex, pageSize);

            _logger.LogDebug(
                "Retrieved {UserCount} dashboards for page {PageIndex}",
                pagedDashboards.Items.Count, pageIndex);

            return View(pagedDashboards);
        }

        [Authorize(Policy = "CanViewDashboards")]
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation(
                "User {UserName} is viewing details for Dashboard Id: {UserId}",
                User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown", id);

            var user = await _dashboardQuery.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning(
                    "Dashboard details not found for UserId: {UserId}", id);
                return NotFound();
            }

            _logger.LogDebug("Dashboard details retrieved successfully for UserId: {UserId}", id);
            return View(user);
        }

        [Authorize(Policy = "CanRegisterDashboards")]
        [Authorize(Policy = "CanViewDashboards")]
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("User {UserName} accessed the Create Dashboard form",
                User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown");
            return View();
        }


        [Authorize(Policy = "CanRegisterDashboards")]
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel user)
        {
            _logger.LogInformation(
                "User {UserName} is creating a new Dashboard with Name: {NewUserName}",
                User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown", user.userName);


            var success = await _dashboardQuery.CreateUserAsync(user);

            if (success)
                return RedirectToAction(nameof(Index));

            _logger.LogError(
                "Failed to create Dashboard {NewUserName} by {UserName}",
                user.userName, User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown");

            ModelState.AddModelError("", "Failed to create Dashboard");
            return View(user);
        }

        [Authorize(Policy = "CanDisableDashboards")]
        [HttpPost]
        public async Task<IActionResult> ToggleUserActivity(Guid userId, bool isActive)
        {
            _logger.LogInformation(
                "User {UserName} is attempting to update activity status for Dashboard Id: {UserId} to {IsActive}",
                User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown", userId, isActive);

            var result = await _dashboardQuery.UpdateUserActivityStatusAsync(userId, isActive);

            if (!result)
            {
                _logger.LogWarning(
                    "Failed to update activity status for Dashboard Id: {UserId}",
                    userId);
                TempData["Error"] = "Failed to update dashboard's activity";
            }
            else
            {
                _logger.LogInformation(
                    "Successfully updated activity status for Dashboard Id: {UserId} to {IsActive}",
                    userId, isActive);
                TempData["Success"] = "Dashboard status updated";
            }

            return RedirectToAction("Index");
        }
    }
}
