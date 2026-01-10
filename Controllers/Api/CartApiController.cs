using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Controllers.Api
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate quantity
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than 0" });
            }

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
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate quantity
            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than 0" });
            }

            // Check if product exists in database
            var product = _context.Products.Find(request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Check if item exists in cart
            var cartItems = _cartService.GetCartItems();
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == request.ProductId);
            if (cartItem == null)
            {
                return NotFound(new { message = "Item not found in cart" });
            }

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
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}

