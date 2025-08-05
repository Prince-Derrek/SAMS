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

        public UserController(IUserQueryService userQueryService, IRegisterUserService registerUserService, IUpdateUserActivityStatus updateUserActivityStatus, IGetUserById getUserById)
        {
            _userQueryService = userQueryService;
            _registerUserService = registerUserService;
            _updateUserActivityStatus = updateUserActivityStatus;
            _getUserById = getUserById;
        }
        [Authorize(Policy = "CanViewUsers")]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            var pagedUsers = await _userQueryService.GetPaginatedUsersAsync(pageIndex, pageSize);
            return View(pagedUsers);
        }


        [Authorize(Policy = "CanViewUserDetails")]
        public async Task<IActionResult> Details(Guid id)
        {
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

            var success = await _registerUserService.CreateUserAsync(user);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create user");
            return View(user);
        }

        [Authorize(Policy = "CanDisableUsers")]
        [HttpPost]
        public async Task<IActionResult> ToggleUserActivity(Guid userId, bool isActive)
        {
            var result = await _updateUserActivityStatus.UpdateUserActivityStatusAsync(userId, isActive);
            if (!result)
            {
                TempData["Error"] = "Failed to update user's activity";
            }
            else
            {
                TempData["Success"] = "User status updated";
            }

            return RedirectToAction("Index");
        }
    }
}
