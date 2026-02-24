using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.Models
{
    // Cette ligne crée l'Index Unique sur le Code-Barre dans la base de données
    [Index(nameof(Barcode), IsUnique = true)]
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)] // On limite la taille pour l'optimisation
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        // Lien avec Category
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // Prix d'achat (Coût)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        // Prix de vente
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; }

        // Alerte si le stock descend sous ce nombre (ex: 5)
        public int MinStockAlert { get; set; }

        public string Unit { get; set; } = "Pcs"; // Pcs, Kg, Litre...
    }
}