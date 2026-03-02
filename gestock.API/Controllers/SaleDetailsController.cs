using gestock.API.Data;
using gestock.API.DTOs;
using gestock.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SaleDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SaleDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailDto>>> GetSaleDetails()
        {
            var details = await _context.SaleDetails
                                        .Include(sd => sd.Product)
                                        .ToListAsync();

            var detailsDto = details.Select(sd => new SaleDetailDto
            {
                ProductId = sd.ProductId,
                ProductName = sd.Product?.Name ?? "Produit supprimé",
                Quantity = sd.Quantity,
                UnitPrice = sd.UnitPrice,
                SubTotal = sd.SubTotal
            }).ToList();

            return Ok(detailsDto);
        }

        // GET: api/SaleDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetail>> GetSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetails
                                           .Include(sd => sd.Product)
                                           .FirstOrDefaultAsync(sd => sd.DetailId == id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            return saleDetail;
        }

        // POST: api/SaleDetails
        [HttpPost]
        public async Task<ActionResult<SaleDetail>> PostSaleDetail(SaleDetail saleDetail)
        {
            _context.SaleDetails.Add(saleDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSaleDetail),
                new { id = saleDetail.DetailId }, saleDetail);
        }

        // PUT: api/SaleDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleDetail(int id, SaleDetail saleDetail)
        {
            if (id != saleDetail.DetailId)
            {
                return BadRequest();
            }

            _context.Entry(saleDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SaleDetails.Any(e => e.DetailId == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/SaleDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetails
                                           .FirstOrDefaultAsync(s => s.DetailId == id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            _context.SaleDetails.Remove(saleDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}