using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperMarcheApp.Views
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }
        private void Articles_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ArticlesView();
        }

        private void Vendeurs_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VendorsView();
        }

        private void Catégories_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CategoriesView();
        }

        private void Facturation_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new FacturationView();
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            LoginView login = new LoginView();
            login.Show();
            this.Close();
        }
        
    }
}
