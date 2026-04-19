using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Data;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Models.Cart;
using PrintingPlatform.Models.Checkout;
using System.Text.Json;

namespace PrintingPlatform.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartSessionKey = "CartSession";
        private readonly PrintingPlatformContext _PrintingPlatformContext;

        public CheckoutController(PrintingPlatformContext PrintingPlatformContext)
        {
            _PrintingPlatformContext = PrintingPlatformContext;
        }

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

            Order orderEntity = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                FullName = viewModel.FullName,
                Email = viewModel.Email,
                Address = viewModel.Address,
                City = viewModel.City,
                PostalCode = viewModel.PostalCode,
                Status = "Pending",
                Subtotal = viewModel.Subtotal,
                ShippingCost = viewModel.ShippingCost,
                Tax = viewModel.Tax,
                Total = viewModel.Total,
                CreatedAt = DateTime.UtcNow,
                Items = cart.Items.Select(cartItemViewModel => new OrderItem
                {
                    ProductId = cartItemViewModel.ProductId,
                    ProductName = cartItemViewModel.ProductName,
                    ImageUrl = cartItemViewModel.ImageUrl,
                    UnitPrice = cartItemViewModel.Price,
                    Quantity = cartItemViewModel.Quantity
                }).ToList()
            };

            _PrintingPlatformContext.Orders.Add(orderEntity);
            _PrintingPlatformContext.SaveChanges();

            HttpContext.Session.Remove(CartSessionKey);

            TempData["CheckoutSuccess"] = $"Your order {orderEntity.OrderNumber} has been placed successfully!";
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

        private static string GenerateOrderNumber()
        {
            return $"PP-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}