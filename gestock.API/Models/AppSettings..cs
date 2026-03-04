using System.ComponentModel.DataAnnotations;

namespace gestock.API.Models
{
    public class AppSettings
    {
        [Key]
        public int Id { get; set; }

        // 🏪 Magasin
        public string StoreName { get; set; } = "SUPERMARCHÉ";
        public string StoreAddress { get; set; } = string.Empty;
        public string StorePhone { get; set; } = string.Empty;

        // 💰 Monnaie
        public string CurrencySymbol { get; set; } = "DA";
        public bool CurrencyAfterAmount { get; set; } = true;  // true = "100 DA", false = "DA 100"

        // 🧾 Facturation
        public string InvoicePrefix { get; set; } = "FAC";
        public string ReceiptFooter { get; set; } = "Merci pour votre achat !";

        // 📦 Stock
        public int DefaultMinStockAlert { get; set; } = 5;
    }
}