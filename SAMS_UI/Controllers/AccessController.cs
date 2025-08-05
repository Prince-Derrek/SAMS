using Microsoft.AspNetCore.Mvc;

namespace SAMS_UI.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Unauthorized()
        {
            return View();
        }
    }
}
