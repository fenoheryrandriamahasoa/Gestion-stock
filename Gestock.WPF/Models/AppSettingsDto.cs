namespace SuperMarcheApp.Models
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

        /// <summary>
        /// Formate un montant avec la devise configurée
        /// Ex: "1 250.00 DA" ou "$ 1,250.00"
        /// </summary>
        public string FormatPrice(decimal amount)
        {
            if (CurrencyAfterAmount)
                return $"{amount:N2} {CurrencySymbol}";
            else
                return $"{CurrencySymbol} {amount:N2}";
        }
    }
}