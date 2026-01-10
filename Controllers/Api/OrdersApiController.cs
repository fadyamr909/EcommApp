using ECommerceApp.Services;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ECommerceApp.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrdersApiController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder()
        {
            var cartItems = _cartService.GetCartItems();

            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest(new { message = "Cart is empty" });
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(cartItems);
                _cartService.ClearCart();
                return Ok(new { orderId = order.Id, message = "Order placed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error placing order", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}

