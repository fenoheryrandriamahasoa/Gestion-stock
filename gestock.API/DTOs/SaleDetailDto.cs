namespace gestock.API.DTOs
{
    public class SaleDetailDto
    {
        public int ProductId { get; set; }       // ✅ Ajouté
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}