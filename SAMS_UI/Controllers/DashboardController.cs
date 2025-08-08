using Microsoft.AspNetCore.Mvc;

namespace SAMS_UI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
