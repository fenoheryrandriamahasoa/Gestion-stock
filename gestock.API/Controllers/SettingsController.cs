using gestock.API.Data;
using gestock.API.DTOs;
using gestock.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Settings
        [HttpGet]
        public async Task<ActionResult<AppSettingsDto>> GetSettings()
        {
            // Il n'y a qu'UNE seule ligne de paramètres (Id = 1)
            var settings = await _context.AppSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                // Créer les paramètres par défaut s'ils n'existent pas
                settings = new AppSettings();
                _context.AppSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            var dto = new AppSettingsDto
            {
                Id = settings.Id,
                StoreName = settings.StoreName,
                StoreAddress = settings.StoreAddress,
                StorePhone = settings.StorePhone,
                CurrencySymbol = settings.CurrencySymbol,
                CurrencyAfterAmount = settings.CurrencyAfterAmount,
                InvoicePrefix = settings.InvoicePrefix,
                ReceiptFooter = settings.ReceiptFooter,
                DefaultMinStockAlert = settings.DefaultMinStockAlert
            };

            return Ok(dto);
        }

        // PUT: api/Settings
        [HttpPut]
        public async Task<IActionResult> PutSettings(AppSettingsDto dto)
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                return NotFound();
            }

            settings.StoreName = dto.StoreName;
            settings.StoreAddress = dto.StoreAddress;
            settings.StorePhone = dto.StorePhone;
            settings.CurrencySymbol = dto.CurrencySymbol;
            settings.CurrencyAfterAmount = dto.CurrencyAfterAmount;
            settings.InvoicePrefix = dto.InvoicePrefix;
            settings.ReceiptFooter = dto.ReceiptFooter;
            settings.DefaultMinStockAlert = dto.DefaultMinStockAlert;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}