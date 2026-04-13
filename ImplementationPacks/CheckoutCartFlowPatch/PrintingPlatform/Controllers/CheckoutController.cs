using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Checkout;

namespace PrintingPlatform.Controllers
{
    public class CheckoutController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new CheckoutViewModel();

            return View("Checkout", viewModel);
        }
    }
}