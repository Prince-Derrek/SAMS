using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SAMS_UI.Controllers
{
    public class AccessController : Controller
    {
        private readonly ILogger<AccessController> _logger;

        public AccessController(ILogger<AccessController> logger)
        {
            _logger = logger;
        }

        public IActionResult Unauthorized()
        {
            _logger.LogInformation("Access Denied");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logging out");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Auth");
            // "Auth" should be the controller that serves your login page
        }
    }
}
