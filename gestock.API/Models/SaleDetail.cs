using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class SaleDetail
    {
        [Key]
        public int DetailID { get; set; }

        public int SaleID { get; set; } // Lien vers la vente globale
        [ForeignKey("SaleID")]
        public Sale? Sale { get; set; }

        public int ProductId { get; set; } // Lien vers le produit
        [ForeignKey("ArticleId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Prix au moment de la vente

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } // Quantity * UnitPrice
    }
}