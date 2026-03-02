using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = "Espèces";

        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}