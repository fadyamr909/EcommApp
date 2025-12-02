using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Product
    {
        public int Id { get; set; }  // Unique identifier for each product

        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Product category is required")]
        public string Category { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }  // Optional: URL or uploaded image

        // Optional timestamps
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
