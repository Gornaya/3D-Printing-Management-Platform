using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Data;
using PrintingPlatform.Models.Product;
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
            var productEntity = _context.Products.FirstOrDefault(productItem => productItem.Id == id);

            if (productEntity == null)
             { return NotFound();
             }

            var viewModel = new ProductDetailsViewModel 
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                DiscountedPrice = productEntity.DiscountedPrice,
                ImageUrl = productEntity.ImageUrl
            };
            return View(viewModel);
        }
    }
}