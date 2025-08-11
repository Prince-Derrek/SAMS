using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;
using System.Security.Claims;

namespace SAMS_UI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserQueryService _userQueryService;
        private readonly IRegisterUserService _registerUserService;
        private readonly IUpdateUserActivityStatus _updateUserActivityStatus;
        private readonly IGetUserById _getUserById;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserQueryService userQueryService, IRegisterUserService registerUserService, IUpdateUserActivityStatus updateUserActivityStatus, IGetUserById getUserById, ILogger<UserController> logger)
        {
            _userQueryService = userQueryService;
            _registerUserService = registerUserService;
            _updateUserActivityStatus = updateUserActivityStatus;
            _getUserById = getUserById;
            _logger = logger;
        }
        [Authorize(Policy = "CanViewUsers")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Calling service to get paginated users");
            var pagedUsers = await _userQueryService.GetPaginatedUsersAsync(pageIndex, pageSize);
            return View(pagedUsers);
        }


        [Authorize(Policy = "CanViewUserDetails")]
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation("Calling service to get user by Id");
            var user = await _getUserById.GetUserByIdAsync(id);
            return View(user);
        } 

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            if (!User.HasClaim("policy", "CanRegisterUsers") && User.HasClaim("policy", "CanViewUsers"))
            {
                _logger.LogWarning("User {UserName} lacks policy to access the create user form", User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown");
                return RedirectToAction("Unauthorized", "Access");
            }
            _logger.LogInformation("User {UserName} accessed the Create User form", User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown");
            return View();
        }

        [Authorize(Policy = "CanRegisterUsers")]
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel user)
        {
            if (!ModelState.IsValid)
                return View(user);

            _logger.LogInformation("Calling service to regiseter a new user");
            var success = await _registerUserService.CreateUserAsync(user);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogInformation("Registration of new user failed");
            }

                ModelState.AddModelError("", "Failed to create user");
            return View(user);
        }

        //Will come back to this later

       /* [Authorize(Policy = "CanDisableUsers")]
        [HttpPost]
        public async Task<IActionResult> ToggleBackendUserActivity(Guid userId, bool isActive)
        {
            _logger.LogInformation("Calling service to update user activity status");
            var result = await _updateUserActivityStatus.UpdateBackendUserActivityStatusAsync(userId, isActive);
            if (!result)
            {
                TempData["Error"] = "Failed to update user's activity";
                _logger.LogInformation("Failed to update user's activity");
            }
            else
            {
                TempData["Success"] = "User status updated";
                _logger.LogInformation("User status updated");
            }

            return RedirectToAction("Index");
        } */
        [Authorize(Policy = "CanDisableUsers")]
        [HttpPost]
        public async Task<IActionResult> ToggleFrontendUserActivity(Guid userId, bool isActive)
        {
            _logger.LogInformation("Calling service to update frontend user activity status");

            var result = await _updateUserActivityStatus.UpdateFrontendUserActivityStatusAsync(userId, isActive);

            if (!result)
            {
                TempData["Error"] = "Failed to update frontend user's activity status.";
                _logger.LogWarning("Frontend user activity update failed for user ID: {UserId}", userId);
            }
            else
            {
                TempData["Success"] = "Frontend user status updated successfully.";
                _logger.LogInformation("Frontend user activity updated for user ID: {UserId}", userId);
            }

            return RedirectToAction("Index");
        }

    }
}
