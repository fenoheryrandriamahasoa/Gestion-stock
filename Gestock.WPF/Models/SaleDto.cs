using System;
using System.Collections.Generic;

namespace SuperMarcheApp.Models
{
    public class SaleDto
    {
        public int SaleId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Vendeur { get; set; } = string.Empty;
        public List<SaleDetailDto> Details { get; set; } = new();
    }

    public class SaleDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    // Pour POST
    public class SaleRequest
    {
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = "Espèces";
        public List<SaleDetailRequest> SaleDetails { get; set; } = new();
    }

    public class SaleDetailRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}