using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Data;
using PrintingPlatform.Models.Order;
using System.Security.Claims;

namespace PrintingPlatform.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly PrintingPlatformContext _printingPlatformContext;
        public OrdersController(PrintingPlatformContext printingPlatformContext)
        {
            _printingPlatformContext = printingPlatformContext;
        }

        [HttpGet]
        public IActionResult Index(string? search, string? status, string? sort = "newest")
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            IQueryable<Data.Entities.Order> ordersQuery = _printingPlatformContext.Orders
                .AsNoTracking()
                .Include(orderEntity => orderEntity.Items)
                .Where(orderEntity => orderEntity.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string normalizedSearch = search.Trim();

                ordersQuery = ordersQuery.Where(orderEntity =>
                    orderEntity.OrderNumber.Contains(normalizedSearch) ||
                    orderEntity.Items.Any(orderItemEntity =>
                        orderItemEntity.ProductName.Contains(normalizedSearch)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                ordersQuery = ordersQuery.Where(orderEntity => orderEntity.Status == status);
            }

            ordersQuery = sort switch
            {
                "oldest" => ordersQuery.OrderBy(orderEntity => orderEntity.CreatedAt),
                "price_desc" => ordersQuery.OrderByDescending(orderEntity => orderEntity.Total),
                "price_asc" => ordersQuery.OrderBy(orderEntity => orderEntity.Total),
                _ => ordersQuery.OrderByDescending(orderEntity => orderEntity.CreatedAt)
            };

            List<UserOrderCardViewModel> userOrderCardViewModels = ordersQuery
                .Select(orderEntity => new UserOrderCardViewModel
                {
                    Id = orderEntity.Id,
                    OrderNumber = orderEntity.OrderNumber,
                    Title = orderEntity.Items
                        .OrderBy(orderItemEntity => orderItemEntity.Id)
                        .Select(orderItemEntity => orderItemEntity.ProductName)
                        .FirstOrDefault() ?? "Order",
                    ImageUrl = orderEntity.Items
                        .OrderBy(orderItemEntity => orderItemEntity.Id)
                        .Select(orderItemEntity => orderItemEntity.ImageUrl)
                        .FirstOrDefault() ?? "/images/placeholder-product.png",
                    Status = orderEntity.Status,
                    Material = "Not specified",
                    Color = "Not specified",
                    Quantity = orderEntity.Items.Sum(orderItemEntity => orderItemEntity.Quantity),
                    TotalPrice = orderEntity.Total,
                    CreatedAt = orderEntity.CreatedAt,
                    CompletedAt = null
                })
                .ToList();

            ViewBag.Search = search;
            ViewBag.Status = status ?? "All";
            ViewBag.Sort = sort;

            return View(userOrderCardViewModels);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var order = _printingPlatformContext.Orders
                .AsNoTracking()
                .Include(orderEntity => orderEntity.Items)
                .FirstOrDefault(orderEntity => orderEntity.Id == id && orderEntity.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}