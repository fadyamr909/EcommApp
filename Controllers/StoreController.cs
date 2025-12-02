using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public StoreController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: Store
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Store/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Store/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            _cartService.AddItem(productId, quantity);
            TempData["SuccessMessage"] = $"{product.Name} (x{quantity}) has been added to your cart!";
            
            // Redirect back to store page so user can continue shopping
            return RedirectToAction(nameof(Index));
        }

        // GET: Store/Cart
        public IActionResult Cart()
        {
            var cartItems = _cartService.GetCartItems();
            ViewBag.Total = _cartService.GetTotal();
            return View(cartItems);
        }

        // POST: Store/UpdateCartItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction(nameof(Cart));
        }

        // POST: Store/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveItem(productId);
            return RedirectToAction(nameof(Cart));
        }
    }
}

