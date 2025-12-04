using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers.Api
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public CartApiController(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var cartItems = _cartService.GetCartItems();
            var total = _cartService.GetTotal();
            return Ok(new { items = cartItems, total });
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            var product = _context.Products.Find(request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            _cartService.AddItem(request.ProductId, request.Quantity);
            return Ok(new { message = "Item added to cart" });
        }

        [HttpPut("update")]
        public IActionResult UpdateCartItem([FromBody] UpdateCartRequest request)
        {
            _cartService.UpdateQuantity(request.ProductId, request.Quantity);
            return Ok(new { message = "Cart updated" });
        }

        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveItem(productId);
            return Ok(new { message = "Item removed from cart" });
        }

        [HttpPost("clear")]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return Ok(new { message = "Cart cleared" });
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

