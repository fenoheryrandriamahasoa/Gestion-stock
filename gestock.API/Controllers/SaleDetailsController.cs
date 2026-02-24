using gestock.API.Data;
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

        // On injecte la base de données ici
        public SaleDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/SaleDetail (Pour avoir la liste de tous les detaild de facture)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaledetails()
        {
            return await _context.SaleDetails
                        // .Include(p => p.Category)
                        .ToListAsync();
        }

        // 2. GET: api/SaleDetail/5 (Pour avoir un seul detail de facture par son ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetail>> GetSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetails.FindAsync(id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            return saleDetail;
        }

        // 3. POST: api/SaleDetail (Pour AJOUTER un detail facture)
        [HttpPost]
        public async Task<ActionResult<SaleDetail>> PostSaleDetail(SaleDetail saleDetail)
        {
            _context.SaleDetails.Add(saleDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSaleDetail), new { id = saleDetail.DetailID }, saleDetail);
        }

        // 4. PUT: api/SaleDetail/5 (Pour MODIFIER un article)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleDetail(int id, SaleDetail saleDetail)
        {
            if (id != saleDetail.DetailID)
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
                if (!_context.SaleDetails.Any(e => e.DetailID == id))
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

        // 5. DELETE: api/SaleDetail/5 (Pour SUPPRIMER un detail facture)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetails
                                        .Include(s => s.Sale)
                                        .FirstOrDefaultAsync(s => s.DetailID == id);

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
