namespace gestock.API.DTOs
{
    public class AppSettingsDto
    {
        public int Id { get; set; }

        // 🏪 Magasin
        public string StoreName { get; set; } = string.Empty;
        public string StoreAddress { get; set; } = string.Empty;
        public string StorePhone { get; set; } = string.Empty;

        // 💰 Monnaie
        public string CurrencySymbol { get; set; } = string.Empty;
        public bool CurrencyAfterAmount { get; set; }

        // 🧾 Facturation
        public string InvoicePrefix { get; set; } = string.Empty;
        public string ReceiptFooter { get; set; } = string.Empty;

        // 📦 Stock
        public int DefaultMinStockAlert { get; set; }
    }
}