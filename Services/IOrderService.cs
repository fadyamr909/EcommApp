using ECommerceApp.Models;
using ECommerceApp.ViewModels;

namespace ECommerceApp.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(List<CartItemViewModel> cartItems);
        Task<Order?> GetOrderAsync(int orderId);
        Task<List<Order>> GetAllOrdersAsync();
    }
}

