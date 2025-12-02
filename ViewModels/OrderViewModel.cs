using ECommerceApp.Models;

namespace ECommerceApp.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => PriceAtPurchase * Quantity;
    }
}

