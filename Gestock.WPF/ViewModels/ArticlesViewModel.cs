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
                    Code = "A001",
                    Nom = "Coca-Cola",
                    Prix = 1.5,
                    Stock = 120,
                    DateExpiration = "12/06/2026",
                    Categorie = "Boissons"
                },
                new Article
                {
                    Code = "A002",
                    Nom = "Lait entier",
                    Prix = 0.9,
                    Stock = 80,
                    DateExpiration = "05/03/2026",
                    Categorie = "Produits laitiers"
                },
                new Article
                {
                    Code = "A003",
                    Nom = "Riz 5kg",
                    Prix = 6.2,
                    Stock = 40,
                    DateExpiration = "01/01/2027",
                    Categorie = "Épicerie"
                }
            };
        }
    }
}
