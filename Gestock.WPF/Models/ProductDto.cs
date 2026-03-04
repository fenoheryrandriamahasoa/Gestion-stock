namespace SuperMarcheApp.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockAlert { get; set; }
        public string Unit { get; set; } = "Pcs";
        public bool IsLowStock => StockQuantity <= MinStockAlert;
        public override string ToString() => Name;
        public string PurchasePriceFormatted => App.FormatPrice(PurchasePrice);
        public string SellingPriceFormatted => App.FormatPrice(SellingPrice);
    }

    // Pour POST/PUT
    public class ProductRequest
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockAlert { get; set; }
        public string Unit { get; set; } = "Pcs";
    }
}