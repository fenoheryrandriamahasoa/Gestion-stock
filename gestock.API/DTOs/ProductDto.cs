namespace gestock.API.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }       // ✅ Ajouté
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }      // ✅ Ajouté
        public string CategoryName { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockAlert { get; set; }
        public string Unit { get; set; } = "Pcs";
        public bool IsLowStock => StockQuantity <= MinStockAlert;  // ✅ Utile pour alertes
    }
}