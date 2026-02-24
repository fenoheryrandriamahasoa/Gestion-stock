using System.Collections.ObjectModel;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.ViewModels
{
    public class CategoriesViewModel
    {
        public ObservableCollection<Category> Categories { get; set; }

        public CategoriesViewModel()
        {
            Categories = new ObservableCollection<Category>
            {
                new Category { Nom = "Boissons" },
                new Category { Nom = "Produits laitiers" },
                new Category { Nom = "Fruits et légumes" },
                new Category { Nom = "Épicerie" },
                new Category { Nom = "Hygiène" }
            };
        }
    }
}
