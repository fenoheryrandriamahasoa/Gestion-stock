using System.Windows.Controls;
using SuperMarcheApp.ViewModels;

namespace SuperMarcheApp.Views
{
    /// <summary>
    /// Logique d'interaction pour CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView : UserControl
    {
        public CategoriesView()
        {
            InitializeComponent();
            DataContext = new CategoriesViewModel();

        }
    }
}
