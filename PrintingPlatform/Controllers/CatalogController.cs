using Microsoft.AspNetCore.Mvc;

namespace PrintingPlatform.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}