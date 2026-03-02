using gestock.API.Data;
using gestock.API.DTOs;
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

        // GET: api/StockMovements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovements()
        {
            var movements = await _context.StockMovements
                                          .Include(m => m.Product)
                                          .OrderByDescending(m => m.MovementDate)
                                          .ToListAsync();

            var movementsDto = movements.Select(m => new StockMovementDto
            {
                MovementId = m.MovementId,
                ProductId = m.ProductId,
                ProductName = m.Product?.Name ?? "Produit supprimé",
                Quantity = m.Quantity,
                MovementDate = m.MovementDate,
                Notes = m.Notes
            }).ToList();

            return Ok(movementsDto);
        }

        // GET: api/StockMovements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockMovementDto>> GetStockMovement(int id)
        {
            var movement = await _context.StockMovements
                                         .Include(m => m.Product)
                                         .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null)
            {
                return NotFound();
            }

            var movementDto = new StockMovementDto
            {
                MovementId = movement.MovementId,
                ProductId = movement.ProductId,
                ProductName = movement.Product?.Name ?? "Produit supprimé",
                Quantity = movement.Quantity,
                MovementDate = movement.MovementDate,
                Notes = movement.Notes
            };

            return Ok(movementDto);
        }

        // POST: api/StockMovements
        [HttpPost]
        public async Task<ActionResult<StockMovement>> PostStockMovement(StockMovement stockMovement)
        {
            var product = await _context.Products.FindAsync(stockMovement.ProductId);

            if (product == null)
            {
                return BadRequest(new { message = "Produit introuvable" });
            }

            // Ajouter au stock
            product.StockQuantity += stockMovement.Quantity;

            stockMovement.MovementDate = DateTime.Now;

            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockMovement),
                new { id = stockMovement.MovementId }, stockMovement);
        }

        // PUT: api/StockMovements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockMovement(int id, StockMovement stockMovement)
        {
            if (id != stockMovement.MovementId)
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
                if (!_context.StockMovements.Any(e => e.MovementId == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/StockMovements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            var movement = await _context.StockMovements.FindAsync(id);

            if (movement == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(movement.ProductId);
            if (product != null)
            {
                product.StockQuantity -= movement.Quantity;
            }

            _context.StockMovements.Remove(movement);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}