using System.Collections.ObjectModel;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.ViewModels
{
    public class ArticlesViewModel
    {
        public ObservableCollection<Article> Articles { get; set; }

        public ArticlesViewModel()
        {
            Articles = new ObservableCollection<Article>
            {
                new Article
                {
                    Barcode = "A001",
                    Name = "Coca-Cola",
                    PurchasePrice = 1.5,
                    StockQuantity = 120,
                    SellingPrice = "12/06/2026",
                    CategoryName = "Boissons"
                },
                new Article
                {
                    Barcode = "A002",
                    Name = "Lait entier",
                    PurchasePrice = 0.9,
                    StockQuantity = 80,
                    SellingPrice = "05/03/2026",
                    CategoryName = "Produits laitiers"
                },
                new Article
                {
                    Barcode = "A003",
                    Name = "Riz 5kg",
                    PurchasePrice = 6.2,
                    StockQuantity = 40,
                    SellingPrice = "01/01/2027",
                    CategoryName = "Épicerie"
                }
            };
        }
    }
}
