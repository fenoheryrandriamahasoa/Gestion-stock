using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class SaleDetail
    {
        [Key]
        // ✅ FIX : DetailID → DetailId (cohérence)
        public int DetailId { get; set; }

        // ✅ FIX : SaleID → SaleId (cohérence)
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]          // ✅ FIX : était "SaleID"
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]       // ❌→✅ CRITICAL FIX : était "ArticleId" !
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
    }
}