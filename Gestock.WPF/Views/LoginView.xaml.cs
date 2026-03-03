using System;
using System.Windows;

namespace SuperMarcheApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Login.IsEnabled = false;
                Login.Content = "Connexion...";

                var user = await App.Api.LoginAsync(username, password);

                if (user != null)
                {
                    // Stocker l'utilisateur connecté
                    App.CurrentUser = user;

                    var mainView = new MainView();
                    mainView.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Échec de connexion : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Login.IsEnabled = true;
                Login.Content = "SE CONNECTER";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}