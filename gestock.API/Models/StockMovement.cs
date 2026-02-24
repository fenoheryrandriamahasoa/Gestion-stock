using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class StockMovement
    {
        [Key]
        public int MovementID { get; set; }

        // Lien avec produit 
        public int ProductId { get; set; }
        // Cette ligne permet à EF Core de comprendre la relation
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; } 
        public DateTime MovementDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty; 
    }
}