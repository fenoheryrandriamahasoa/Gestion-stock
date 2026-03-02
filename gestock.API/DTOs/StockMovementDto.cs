namespace gestock.API.DTOs
{
    public class StockMovementDto
    {
        public int MovementId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}