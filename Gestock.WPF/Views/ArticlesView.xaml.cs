using System.Windows.Controls;
using SuperMarcheApp.ViewModels;

namespace SuperMarcheApp.Views
{
    /// <summary>
    /// Logique d'interaction pour ArticlesView.xaml
    /// </summary>
    public partial class ArticlesView : UserControl
    {
        public ArticlesView()
        {
            InitializeComponent();
            DataContext = new ArticlesViewModel();
        }
    }
}
