using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAMS_UI.Services.Interfaces;
using SAMS_UI.ViewModels;

namespace SAMS_UI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserQueryService _userQueryService;
        private readonly IRegisterUserService _registerUserService;
        private readonly IUpdateUserActivityStatus _updateUserActivityStatus;
        private readonly IGetUserById _getUserById;
        private readonly ILogger<UserController> _logger

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

        [Authorize(Policy = "CanRegisterUsers")]
        [HttpGet]
        public IActionResult Create()
        {
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

        [Authorize(Policy = "CanDisableUsers")]
        [HttpPost]
        public async Task<IActionResult> ToggleUserActivity(Guid userId, bool isActive)
        {
            _logger.LogInformation("Calling service to update user activity status");
            var result = await _updateUserActivityStatus.UpdateUserActivityStatusAsync(userId, isActive);
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
        }
    }
}
