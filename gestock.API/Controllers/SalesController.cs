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

        // GET: api/Sale
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            // On récupère les données
            var sales = await _context.Sales
                                .Include(s => s.User)
                                .Include(s => s.SaleDetails)
                                .ThenInclude(sd => sd.Product)
                                .ToListAsync();

            // On transforme (Map) les données en DTO
            var salesDto = sales.Select(s => new SaleDto
            {
                SaleID = s.SaleID,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Vendeur = s.User != null ? s.User.Username : "Inconnu",
               
                // On transforme la liste des détails
                Details = s.SaleDetails.Select(sd => new SaleDetailDto
                {
                    ProductName = sd.Product != null ? sd.Product.Name : "Produit supprimé",
                    Quantity = sd.Quantity,
                    UnitPrice = sd.UnitPrice,
                    SubTotal = sd.SubTotal
                }).ToList()
            }).ToList();

            return salesDto;
        }

        // GET: api/Sale/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _context.Sales
                                          .Include(s => s.User)
                                         .Include(s => s.SaleDetails)
                                         .ThenInclude(sd => sd.Product)
                                         .FirstOrDefaultAsync(s => s.SaleID == id);


            if (sale == null)
            {
                return NotFound();
            }

            return sale;
        }

        // POST: api/Sale
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            _context.Sales.Add(sale);
            //  Mettre à jour le stock des produits
            foreach (var detail in sale.SaleDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= detail.Quantity; // On diminue le stock
                }
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.SaleID }, sale);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, Sale sale)
        {
            if (id != sale.SaleID)
            {
                return BadRequest();
            }

            _context.Entry(sale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Sales.Any(e => e.SaleID == id))
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

        // DELETE: api/Csle/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
