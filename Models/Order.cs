using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

