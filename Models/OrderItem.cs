using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}

