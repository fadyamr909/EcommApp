using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ECommerceApp.Controllers.Api
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsApiController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? category)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(category))
            {
                // Case-insensitive filter by category
                var normalized = category.Trim().ToLower();
                query = query.Where(p => p.Category.ToLower() == normalized);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            // Find the existing product in the database
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Basic model validation (in case required fields are missing)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update allowed fields
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Category = updatedProduct.Category;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound(new { message = "Product not found" });
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                // Check if product is referenced by any order items - do this check first
                var orderItemsCount = await _context.OrderItems
                    .CountAsync(oi => oi.ProductId == id);
                    
                if (orderItemsCount > 0)
                {
                    return BadRequest(new { 
                        message = $"Cannot delete product that is referenced in {orderItemsCount} existing order(s)"
                    });
                }

                // Delete associated image file if exists (don't fail if image deletion fails)
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    try
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch
                    {
                        // Ignore image deletion errors - continue with product deletion
                    }
                }

                // Remove product from database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                // Check all levels of inner exceptions for foreign key constraint
                Exception? exception = dbEx;
                while (exception != null)
                {
                    var message = exception.Message;
                    
                    // Check message for constraint violations
                    if (message.Contains("REFERENCE constraint") || 
                        message.Contains("FOREIGN KEY") || 
                        message.Contains("FK_OrderItems_Products_ProductId"))
                    {
                        return BadRequest(new { message = "Cannot delete product that is referenced in existing orders" });
                    }
                    
                    // Check if it's a SqlException with error code 547
                    if (exception is SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547 || 
                            sqlEx.Message.Contains("REFERENCE constraint") || 
                            sqlEx.Message.Contains("FK_OrderItems_Products_ProductId"))
                        {
                            return BadRequest(new { message = "Cannot delete product that is referenced in existing orders" });
                        }
                    }
                    
                    exception = exception.InnerException;
                }
                return StatusCode(500, new { message = "Database error while deleting product", error = dbEx.Message });
            }
            catch (SqlException sqlEx)
            {
                // SQL Server error code 547 is foreign key constraint violation
                if (sqlEx.Number == 547 || 
                    sqlEx.Message.Contains("REFERENCE constraint") || 
                    sqlEx.Message.Contains("FOREIGN KEY") || 
                    sqlEx.Message.Contains("FK_OrderItems_Products_ProductId"))
                {
                    return BadRequest(new { message = "Cannot delete product that is referenced in existing orders" });
                }
                return StatusCode(500, new { message = "Database error while deleting product", error = sqlEx.Message });
            }
            catch (Exception ex)
            {
                // Check all levels of inner exceptions
                Exception? exception = ex;
                while (exception != null)
                {
                    var message = exception.Message;
                    if (message.Contains("REFERENCE constraint") || 
                        message.Contains("FOREIGN KEY") || 
                        message.Contains("FK_OrderItems_Products_ProductId") ||
                        message.Contains("SqlException"))
                    {
                        return BadRequest(new { message = "Cannot delete product that is referenced in existing orders" });
                    }
                    
                    // Check if inner exception is SqlException
                    if (exception is SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547 || 
                            sqlEx.Message.Contains("REFERENCE constraint") || 
                            sqlEx.Message.Contains("FK_OrderItems_Products_ProductId"))
                        {
                            return BadRequest(new { message = "Cannot delete product that is referenced in existing orders" });
                        }
                    }
                    
                    exception = exception.InnerException;
                }
                return StatusCode(500, new { message = "Error deleting product", error = ex.Message });
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}


