using Microsoft.AspNetCore.Mvc;

namespace PrintingPlatform.Controllers
{
    public class PricingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}