using ECommerceApp.Services;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrdersController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // POST: Orders/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var cartItems = _cartService.GetCartItems();

            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before placing an order.";
                return RedirectToAction("Cart", "Store");
            }

            try
            {
                // Create and save the order transaction
                var order = await _orderService.CreateOrderAsync(cartItems);
                
                // Only clear cart if order was successfully saved
                _cartService.ClearCart();

                TempData["SuccessMessage"] = $"Order #{order.Id} has been placed successfully!";
                return RedirectToAction(nameof(OrderSummary), new { orderId = order.Id });
            }
            catch (Exception)
            {
                // Log the exception (in production, use proper logging)
                TempData["ErrorMessage"] = "An error occurred while placing your order. Please try again.";
                return RedirectToAction("Cart", "Store");
            }
        }

        // GET: Orders/OrderSummary/5
        public async Task<IActionResult> OrderSummary(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown Product",
                    PriceAtPurchase = oi.PriceAtPurchase,
                    Quantity = oi.Quantity
                }).ToList()
            };

            // Calculate subtotal and tax (assuming 10% tax was applied)
            orderViewModel.Subtotal = orderViewModel.OrderItems.Sum(oi => oi.Subtotal);
            orderViewModel.Tax = orderViewModel.TotalAmount - orderViewModel.Subtotal;

            return View(orderViewModel);
        }
    }
}

