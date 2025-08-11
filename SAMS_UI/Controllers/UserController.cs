using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.Models;
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
        private readonly AppDbContext _context;

        public UserController(IUserQueryService userQueryService, IRegisterUserService registerUserService, IUpdateUserActivityStatus updateUserActivityStatus, IGetUserById getUserById, ILogger<UserController> logger, AppDbContext context)
        {
            _userQueryService = userQueryService;
            _registerUserService = registerUserService;
            _updateUserActivityStatus = updateUserActivityStatus;
            _getUserById = getUserById;
            _logger = logger;
            _context = context;
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
            var model = new RegisterViewModel
            {
                AvailableRoles = _context.Roles
                    .Select(r => new Role { Id = r.Id, Name = r.Name })
                    .ToList()
            };

            return View(model);
        }


        [Authorize(Policy = "CanRegisterUsers")]
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel user)
        {
            if (!ModelState.IsValid)
            {
                user.AvailableRoles = _context.Roles
                    .Select(r => new Role { Id = r.Id, Name = r.Name })
                    .ToList();
                return View(user);
            }

            _logger.LogInformation("Calling service to register a new user");
            var success = await _registerUserService.CreateUserAsync(user);

            if (success)
                return RedirectToAction(nameof(Index));

            _logger.LogInformation("Registration of new user failed");

            ModelState.AddModelError("", "Failed to create user");
            user.AvailableRoles = _context.Roles
                .Select(r => new Role { Id = r.Id, Name = r.Name })
                .ToList();
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
