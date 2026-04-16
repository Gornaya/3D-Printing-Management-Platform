using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PrintingPlatform.Data;
using PrintingPlatform.Models.Cart;
using System.Text.Json;

namespace PrintingPlatform.Controllers

{
    public class CartController : Controller
    {

        private const string CartSessionKey = "CartSession";
        private readonly PrintingPlatformContext _context;

        public CartController(PrintingPlatformContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int productID, string?
        returnUrl, bool buyNow = false)
        {
            var product = _context.Products.FirstOrDefault(productItem =>
            productItem.Id == productID);

            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCartFromSession();
            var existingItem = cart.Items.FirstOrDefault(cartItem =>
            cartItem.ProductId == productID);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            SetCartInSession(cart);

            if (buyNow)
            {
                return RedirectToAction("Index", "Checkout");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productID, string? returnUrl)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(cartItem =>
            cartItem.ProductId == productID);

            if (item != null)
            {
                cart.Items.Remove(item);
                SetCartInSession(cart);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity, string? returnUrl)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(cartItem =>
            cartItem.ProductId == productId);

            if (item == null)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction(nameof(Index));
            }

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            SetCartInSession(cart);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrWhiteSpace(cartJson))
            {
                return new CartViewModel();
            }

            return JsonSerializer.Deserialize<CartViewModel>(cartJson)
            ?? new CartViewModel();
        }

        private void SetCartInSession(CartViewModel cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}