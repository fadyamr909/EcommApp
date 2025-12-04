
using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ECommerceApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }

                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }

                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Check if product is referenced by any order items
                var orderItemsCount = await _context.OrderItems
                    .CountAsync(oi => oi.ProductId == id);
                    
                if (orderItemsCount > 0)
                {
                    TempData["ErrorMessage"] = $"Cannot delete product that is referenced in {orderItemsCount} existing order(s).";
                    return RedirectToAction(nameof(Index));
                }

                // Delete associated image file
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

                _context.Products.Remove(product);
                var result = await _context.SaveChangesAsync();
                
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Product was not deleted. No changes were saved.";
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Check all levels of inner exceptions for foreign key constraint
                Exception? exception = dbEx;
                bool isForeignKeyError = false;
                
                while (exception != null)
                {
                    var message = exception.Message;
                    
                    if (message.Contains("REFERENCE constraint") || 
                        message.Contains("FOREIGN KEY") || 
                        message.Contains("FK_OrderItems_Products_ProductId"))
                    {
                        isForeignKeyError = true;
                        break;
                    }
                    
                    if (exception is SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547 || 
                            sqlEx.Message.Contains("REFERENCE constraint") || 
                            sqlEx.Message.Contains("FK_OrderItems_Products_ProductId"))
                        {
                            isForeignKeyError = true;
                            break;
                        }
                    }
                    
                    exception = exception.InnerException;
                }
                
                if (isForeignKeyError)
                {
                    TempData["ErrorMessage"] = "Cannot delete product that is referenced in existing orders.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"An error occurred while deleting the product: {dbEx.Message}";
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547 || 
                    sqlEx.Message.Contains("REFERENCE constraint") || 
                    sqlEx.Message.Contains("FK_OrderItems_Products_ProductId"))
                {
                    TempData["ErrorMessage"] = "Cannot delete product that is referenced in existing orders.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Database error: {sqlEx.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
