using System.Windows;

namespace SuperMarcheApp.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            // Afficher le rôle dans le titre
            if (App.CurrentUser != null)
            {
                Title = $"Gestion Supermarché — {App.CurrentUser.Username} ({App.CurrentUser.Role})";
            }
        }

        private void Articles_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ArticlesView();
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            // Seul l'admin peut gérer les utilisateurs
            if (App.CurrentUser?.Role != "Admin")
            {
                MessageBox.Show("Accès réservé aux administrateurs.",
                    "Accès refusé", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MainContent.Content = new UsersView();
        }

        private void Catégories_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CategoriesView();
        }

        private void Approvisionnement_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ApprovisionnementView();
        }

        private void PointDeVentes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PointDeVenteView();
        }

        private void Facturation_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HistoriqueView();
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Voulez-vous vous déconnecter ?",
                "Déconnexion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                var login = new LoginView();
                login.Show();
                this.Close();
            }
        }
    }
}