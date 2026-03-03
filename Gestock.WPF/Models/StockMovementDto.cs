using System;

namespace SuperMarcheApp.Models
{
    public class StockMovementDto
    {
        public int MovementId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public override string ToString() => $"{ProductName} (+{Quantity})";
    }

    // Pour POST
    public class StockMovementRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}