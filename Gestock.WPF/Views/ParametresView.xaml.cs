using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class ParametresView : UserControl
    {
        private bool _isLoading = true;  // Éviter les événements pendant le chargement

        public ParametresView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadSettings();
        }

        // ══════════════════════════════════════
        //  CHARGEMENT
        // ══════════════════════════════════════

        private async Task LoadSettings()
        {
            try
            {
                _isLoading = true;

                var settings = await App.Api.GetSettingsAsync();

                // Magasin
                txtStoreName.Text = settings.StoreName;
                txtStoreAddress.Text = settings.StoreAddress;
                txtStorePhone.Text = settings.StorePhone;

                // Devise — Chercher dans la liste
                bool found = false;
                for (int i = 0; i < cboCurrency.Items.Count; i++)
                {
                    if (cboCurrency.Items[i] is ComboBoxItem item &&
                        item.Content.ToString() == settings.CurrencySymbol)
                    {
                        cboCurrency.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }

                // Si pas trouvé dans la liste → saisie manuelle
                if (!found)
                {
                    cboCurrency.SelectedIndex = -1;
                    txtCurrencyManual.Text = settings.CurrencySymbol;
                }

                // Position
                cboCurrencyPosition.SelectedIndex = settings.CurrencyAfterAmount ? 0 : 1;

                // Facturation
                txtInvoicePrefix.Text = settings.InvoicePrefix;
                txtReceiptFooter.Text = settings.ReceiptFooter;

                // Stock
                txtDefaultAlert.Text = settings.DefaultMinStockAlert.ToString();

                _isLoading = false;

                // Mettre à jour les aperçus
                UpdatePreview();
                UpdateInvoicePreview();
            }
            catch (Exception ex)
            {
                _isLoading = false;
                MessageBox.Show($"Erreur de chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════
        //  DEVISE — ÉVÉNEMENTS
        // ══════════════════════════════════════

        //  Quand on choisit dans la liste → vider la saisie manuelle
        private void CboCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;

            if (cboCurrency.SelectedItem is ComboBoxItem item)
            {
                txtCurrencyManual.Text = "";  // Vider la saisie manuelle
                UpdatePreview();
            }
        }

        // Quand on tape manuellement → désélectionner la liste
        private void TxtCurrencyManual_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoading) return;

            if (!string.IsNullOrEmpty(txtCurrencyManual.Text.Trim()))
            {
                cboCurrency.SelectedIndex = -1;  // Désélectionner la liste
            }
            UpdatePreview();
        }

        // Position changée
        private void CboCurrencyPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            UpdatePreview();
        }

        // ══════════════════════════════════════
        //  OBTENIR LE SYMBOLE ACTUEL
        // ══════════════════════════════════════

        private string GetCurrentCurrencySymbol()
        {
            // Priorité : saisie manuelle > liste
            var manual = txtCurrencyManual.Text.Trim();
            if (!string.IsNullOrEmpty(manual))
            {
                return manual;
            }

            if (cboCurrency.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() ?? "?";
            }

            return "?";
        }

        // ══════════════════════════════════════
        //  APERÇUS
        // ══════════════════════════════════════

        private void UpdatePreview()
        {
            if (lblPreviewPrice == null || cboCurrencyPosition == null) return;

            var symbol = GetCurrentCurrencySymbol();
            var afterAmount = cboCurrencyPosition.SelectedIndex == 0;

            // Mettre à jour l'aperçu principal
            lblPreviewPrice.Text = afterAmount
                ? $"1 250.00 {symbol}"
                : $"{symbol} 1,250.00";

            // Mettre à jour les textes du ComboBox position
            if (optAfter != null)
                optAfter.Content = $"Après le montant (100.00 {symbol})";
            if (optBefore != null)
                optBefore.Content = $"Avant le montant ({symbol} 100.00)";
        }

        private void UpdateInvoicePreview()
        {
            var prefix = txtInvoicePrefix.Text.Trim();
            if (string.IsNullOrEmpty(prefix)) prefix = "FAC";
            lblPreviewInvoice.Text = $"Aperçu : {prefix}-{DateTime.Now:yyyy}-0001";
        }

        private void TxtInvoicePrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoading) return;
            UpdateInvoicePreview();
        }

        // ══════════════════════════════════════
        //  SAUVEGARDER
        // ══════════════════════════════════════

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show("Le nom du magasin est obligatoire.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currencySymbol = GetCurrentCurrencySymbol();
            if (currencySymbol == "?")
            {
                MessageBox.Show("Veuillez choisir ou saisir un symbole de devise.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtDefaultAlert.Text, out var alertValue) || alertValue < 0)
            {
                MessageBox.Show("Le seuil d'alerte doit être un nombre positif.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var settings = new AppSettingsDto
                {
                    Id = 1,
                    StoreName = txtStoreName.Text.Trim(),
                    StoreAddress = txtStoreAddress.Text.Trim(),
                    StorePhone = txtStorePhone.Text.Trim(),
                    CurrencySymbol = currencySymbol,
                    CurrencyAfterAmount = cboCurrencyPosition.SelectedIndex == 0,
                    InvoicePrefix = txtInvoicePrefix.Text.Trim(),
                    ReceiptFooter = txtReceiptFooter.Text.Trim(),
                    DefaultMinStockAlert = alertValue
                };

                await App.Api.SaveSettingsAsync(settings);

                // Mettre à jour les paramètres globaux
                App.Settings = settings;

                // Mettre à jour le nom dans la MainView (fenêtre parent)
                var mainWindow = Window.GetWindow(this) as MainView;
                if (mainWindow != null)
                {
                    mainWindow.RefreshStoreName();
                }

                MessageBox.Show(" Paramètres enregistrés avec succès !",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}