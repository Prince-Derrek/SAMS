using Microsoft.AspNetCore.Mvc;
using SAMS_UI.DTOs;

namespace SAMS_UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginDTO dto)
        {
            _logger.LogDebug("Calling authentication service");

            var principal = await _authService.AuthenticateAndSignInAsync(dto, HttpContext);

            if (principal == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                _logger.LogWarning("Invalid Login");
                return View(dto);
            }

            _logger.LogInformation("Login Successful");
            return RedirectToAction("Index", "Dashboard");
        }
    }

}
