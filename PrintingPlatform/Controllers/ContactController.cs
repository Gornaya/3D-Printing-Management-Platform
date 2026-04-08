using Microsoft.AspNetCore.Mvc;

namespace PrintingPlatform.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}