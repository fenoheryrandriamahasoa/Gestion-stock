using gestock.API.Data;
using gestock.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockMovementsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Product (Pour avoir la liste de tous les articles)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovement>>> GetStockMovements()
        {
            return await _context.StockMovements
                        // .Include(p => p.Category)
                        .ToListAsync();
        }

        // 2. GET: api/Product/5 (Pour avoir un seul article par son ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<StockMovement>> GetStockMovement(int id)
        {
            var StockMovement = await _context.StockMovements.FindAsync(id);

            if (StockMovement == null)
            {
                return NotFound();
            }

            return StockMovement;
        }

        // 3. POST: api/Product (Pour AJOUTER un article)
        [HttpPost]
        public async Task<ActionResult<StockMovement>> PostStockMovement(StockMovement stockMovement)
        {
            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockMovement), new { id = stockMovement.MovementID }, stockMovement);
        }

        // 4. PUT: api/StockMovements/5 (Pour MODIFIER un article)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockMovement(int id, StockMovement stockMovement)
        {
            if (id != stockMovement.MovementID)
            {
                return BadRequest();
            }

            _context.Entry(stockMovement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StockMovements.Any(e => e.MovementID == id))
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
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            var stockMovement = await _context.StockMovements
                                        .Include(s => s.Product)
                                        .FirstOrDefaultAsync(s => s.MovementID == id);

            if (stockMovement == null)
            {
                return NotFound();
            }

            _context.StockMovements.Remove(stockMovement);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
