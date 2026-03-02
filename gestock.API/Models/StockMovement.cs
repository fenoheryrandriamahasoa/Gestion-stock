using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class StockMovement
    {
        [Key]
        public int MovementId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
    }
}