using ECommerceApp.Data;
using ECommerceApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ECommerceApp.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private Dictionary<int, int> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return new Dictionary<int, int>();

            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new Dictionary<int, int>();

            return JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();
        }

        private void SaveCart(Dictionary<int, int> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return;

            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }

        public void AddItem(int productId, int quantity)
        {
            var cart = GetCart();
            if (cart.ContainsKey(productId))
            {
                cart[productId] += quantity;
            }
            else
            {
                cart[productId] = quantity;
            }
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            if (quantity <= 0)
            {
                RemoveItem(productId);
                return;
            }

            if (cart.ContainsKey(productId))
            {
                cart[productId] = quantity;
                SaveCart(cart);
            }
        }

        public void RemoveItem(int productId)
        {
            var cart = GetCart();
            cart.Remove(productId);
            SaveCart(cart);
        }

        public List<CartItemViewModel> GetCartItems()
        {
            var cart = GetCart();
            var cartItems = new List<CartItemViewModel>();

            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.Key);
                if (product != null)
                {
                    cartItems.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = item.Value,
                        ImageUrl = product.ImageUrl
                    });
                }
            }

            return cartItems;
        }

        public decimal GetTotal()
        {
            return GetCartItems().Sum(item => item.Subtotal);
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(CartSessionKey);
            }
        }

        public int GetCartItemCount()
        {
            var cart = GetCart();
            return cart.Values.Sum();
        }
    }
}

