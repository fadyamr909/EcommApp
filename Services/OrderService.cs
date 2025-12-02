using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(List<CartItemViewModel> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cart items cannot be empty", nameof(cartItems));
            }

            // Use database transaction to ensure all-or-nothing save
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Calculate subtotal
                var subtotal = cartItems.Sum(item => item.Subtotal);

                // Apply pricing logic: Add 10% tax
                // Alternative options:
                // Option B: Apply fixed 5% discount: var totalAmount = subtotal * 0.95m;
                // Option C: Add +20 EGP markup per product: var totalAmount = subtotal + (cartItems.Count * 20);
                var tax = subtotal * 0.10m;
                var totalAmount = subtotal + tax;

                // Create order
                var order = new Order
                {
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        PriceAtPurchase = cartItem.Price,
                        Quantity = cartItem.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                // Commit transaction - all data is saved successfully
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                // Rollback transaction if anything fails
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}

