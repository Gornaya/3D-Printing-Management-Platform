using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Cart;
using PrintingPlatform.Models.Checkout;
using System.Text.Json;

namespace PrintingPlatform.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartSessionKey = "CartSession";

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession();

            if (cart.Items == null || !cart.Items.Any())
            {
                TempData["CheckoutError"] = "Your cart is empty. Please add items to your cart before proceeding to checkout.";
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = BuildCheckoutViewModel(cart);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel viewModel)
        {
            var cart = GetCartFromSession();

            if (cart.Items == null || !cart.Items.Any())
            {
                TempData["CheckoutError"] = "Your cart is empty. Please add items to your cart before proceeding to checkout.";
                return RedirectToAction("Index", "Cart");
            }

            PopulateCartSummary(viewModel, cart);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            HttpContext.Session.Remove(CartSessionKey);
            TempData["CheckoutSuccess"] = "Your order has been placed successfully!";
            return RedirectToAction("Index", "Catalog");
        }

        private CheckoutViewModel BuildCheckoutViewModel(CartViewModel cart)
        {
            var viewModel = new CheckoutViewModel();
            PopulateCartSummary(viewModel, cart);
            return viewModel;
        }

        private static void PopulateCartSummary(CheckoutViewModel viewModel, CartViewModel cart)
        {
            viewModel.CartItems = cart.Items?.ToList() ?? new List<CartItemViewModel>();
            viewModel.Subtotal = cart.TotalPrice;
            viewModel.ShippingCost = 0m;
            viewModel.Tax = viewModel.Subtotal * 0.10m;
            viewModel.Total = viewModel.Subtotal + viewModel.ShippingCost + viewModel.Tax;
        }

        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrWhiteSpace(cartJson))
            {
                return new CartViewModel
                {
                    Items = new List<CartItemViewModel>()
                };
            }

            var cart = JsonSerializer.Deserialize<CartViewModel>(cartJson);

            return cart ?? new CartViewModel
            {
                Items = new List<CartItemViewModel>()
            };
        }
    }
}