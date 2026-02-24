using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty; // FAC-2024-001

        // Qui a vendu ?
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        // "decimal(18,2)" est important pour l'argent (2 chiffres après la virgule)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = "Espèces"; // Espèces, Carte...

        // Liste des produits vendus dans cette vente
        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}