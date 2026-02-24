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

        // On injecte la base de données ici
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Product (Pour avoir la liste de tous les articles)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var Product = await _context.Products
                        .Include(p => p.Category)
                        .ToListAsync();

            var ProductDto = Product.Select(p => new ProductDto
            {
                Barcode = p.Barcode,
                Name = p.Name,
                PurchasePrice = p.PurchasePrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                MinStockAlert = p.MinStockAlert,
                Unit = p.Unit,
                CategoryName = p.Category != null ? p.Category.Name : "Non classé"
            }).ToList();
            return Ok(ProductDto);
        }

        // 2. GET: api/Product/5 (Pour avoir un seul article par son ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var Product = await _context.Products.FindAsync(id);

            if (Product == null)
            {
                return NotFound();
            }

            return Product;
        }

        // 3. POST: api/Product (Pour AJOUTER un article)
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // 4. PUT: api/Product/5 (Pour MODIFIER un produit)
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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 5. DELETE: api/Product/5 (Pour SUPPRIMER un article)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

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