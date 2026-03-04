using gestock.API.Data;
using gestock.API.DTOs;
using gestock.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            var sales = await _context.Sales
                                      .Include(s => s.User)
                                      .Include(s => s.SaleDetails)
                                      .ThenInclude(sd => sd.Product)
                                    
                                      .ToListAsync();

            var salesDto = sales.Select(s => new SaleDto
            {
                SaleId = s.SaleId,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Vendeur = s.User?.Username ?? "Inconnu",

                Details = s.SaleDetails.Select(sd => new SaleDetailDto
                {
                    ProductId = sd.ProductId,
                    ProductName = sd.Product?.Name ?? "Produit supprimé",
                    Quantity = sd.Quantity,
                    UnitPrice = sd.UnitPrice,
                    SubTotal = sd.SubTotal
                }).ToList()
            }).ToList();

            return Ok(salesDto);
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            var sale = await _context.Sales
                                     .Include(s => s.User)
                                     .Include(s => s.SaleDetails)
                                     .ThenInclude(sd => sd.Product)
                                     .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            var saleDto = new SaleDto
            {
                SaleId = sale.SaleId,
                InvoiceNumber = sale.InvoiceNumber,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                PaymentMethod = sale.PaymentMethod,
                Vendeur = sale.User?.Username ?? "Inconnu",
                Details = sale.SaleDetails.Select(sd => new SaleDetailDto
                {
                    ProductId = sd.ProductId,
                    ProductName = sd.Product?.Name ?? "Produit supprimé",
                    Quantity = sd.Quantity,
                    UnitPrice = sd.UnitPrice,
                    SubTotal = sd.SubTotal
                }).ToList()
            };

            return Ok(saleDto);
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            foreach (var detail in sale.SaleDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);

                if (product == null)
                {
                    return BadRequest(new { message = $"Produit ID {detail.ProductId} introuvable" });
                }

                if (product.StockQuantity < detail.Quantity)
                {
                    return BadRequest(new
                    {
                        message = $"Stock insuffisant pour '{product.Name}'. " +
                                  $"Disponible: {product.StockQuantity}, Demandé: {detail.Quantity}"
                    });
                }

                detail.UnitPrice = product.SellingPrice;
                detail.SubTotal = detail.Quantity * detail.UnitPrice;

                // Diminuer le stock
                product.StockQuantity -= detail.Quantity;
            }

            sale.TotalAmount = sale.SaleDetails.Sum(d => d.SubTotal);

            if (string.IsNullOrEmpty(sale.InvoiceNumber))
            {
                // Utiliser le préfixe des paramètres
                var settings = await _context.AppSettings.FirstOrDefaultAsync();
                var prefix = settings?.InvoicePrefix ?? "FAC";
                var count = await _context.Sales.CountAsync() + 1;
                sale.InvoiceNumber = $"{prefix}-{DateTime.Now:yyyy}-{count:D4}";
            }

            sale.SaleDate = DateTime.Now;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale),
                new { id = sale.SaleId }, sale);
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales
                                     .Include(s => s.SaleDetails)
                                     .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            foreach (var detail in sale.SaleDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    product.StockQuantity += detail.Quantity;
                }
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}