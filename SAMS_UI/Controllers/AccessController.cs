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
    }
}
