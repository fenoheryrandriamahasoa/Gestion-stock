using gestock.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestock.API.DTOs
{
    public class ProductDto
    {
        public string Barcode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }

        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; }

        public int MinStockAlert { get; set; }

        public string Unit { get; set; } = "Pcs";

        public string CategoryName { get; set; } = "non classe";
    }
}
