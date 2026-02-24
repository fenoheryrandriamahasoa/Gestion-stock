namespace gestock.API.DTOs
{
    public class SaleDto
    {
        public int SaleID { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Vendeur { get; set; } = "inconnu";

        public List<SaleDetailDto> Details { get; set; } = new List<SaleDetailDto>();
    }
}