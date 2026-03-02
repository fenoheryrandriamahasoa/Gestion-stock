using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestock.API.Data;
using gestock.API.Models;
using gestock.API.DTOs;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .ToListAsync();

            
            var productsDto = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Barcode = p.Barcode,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : "Non classé",
                PurchasePrice = p.PurchasePrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                MinStockAlert = p.MinStockAlert,
                Unit = p.Unit
            }).ToList();

            return Ok(productsDto);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                Barcode = product.Barcode,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Non classé",
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                StockQuantity = product.StockQuantity,
                MinStockAlert = product.MinStockAlert,
                Unit = product.Unit
            };

            return Ok(productDto);
        }

        
        // GET: api/Products/barcode/123456
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
            {
                return NotFound(new { message = "Produit non trouvé" });
            }

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                Barcode = product.Barcode,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Non classé",
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                StockQuantity = product.StockQuantity,
                MinStockAlert = product.MinStockAlert,
                Unit = product.Unit
            };

            return Ok(productDto);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            
            if (await _context.Products.AnyAsync(p => p.Barcode == product.Barcode))
            {
                return Conflict(new { message = "Ce code-barres existe déjà" });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct),
                new { id = product.ProductId }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.ProductId == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}