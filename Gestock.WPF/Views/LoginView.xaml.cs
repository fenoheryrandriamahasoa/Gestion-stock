using System;
using System.Windows;
using System.Windows.Input;

namespace SuperMarcheApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                txtUsername.Focus();

                // Charger le nom du magasin même avant la connexion
                try
                {
                    var settings = await App.Api.GetSettingsAsync();
                    App.Settings = settings;
                    lblLoginStoreName.Text = settings.StoreName;
                    Title = $"Connexion — {settings.StoreName}";
                }
                catch
                {
                    // API pas encore disponible → garder le nom par défaut
                }
            };
        }

        //  Entrée → soumettre le formulaire
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
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