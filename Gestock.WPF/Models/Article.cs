using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarcheApp.Models
{
    public class Article
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public double PurchasePrice { get; set; }
        public int StockQuantity { get; set; }
        public string SellingPrice { get; set; }
        public string CategoryName { get; set; }
    }
}
