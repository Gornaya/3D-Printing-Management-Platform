using Microsoft.AspNetCore.Mvc;

namespace PrintingPlatform.Controllers

{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}