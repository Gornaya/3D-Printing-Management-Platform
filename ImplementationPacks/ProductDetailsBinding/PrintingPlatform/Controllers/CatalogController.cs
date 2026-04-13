using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Data;
using System.Linq;


namespace PrintingPlatform.Controllers
{
    public class CatalogController : Controller
    {
        private readonly PrintingPlatformContext _context;
        public CatalogController(PrintingPlatformContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.FirstOrDefault(productItem => productItem.Id == id);

            if (product == null)
             { return NotFound();
             }

            return View(product);
        }
    }
}