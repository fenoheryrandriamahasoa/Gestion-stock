namespace SuperMarcheApp.Models
{
    public class SaleLineItem
    {
        public int LineNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
    }
}