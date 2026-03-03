using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class HistoriqueView : UserControl
    {
        private List<SaleDto> _allSales = new();
        private List<SaleDto> _filteredSales = new();

        public HistoriqueView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadData();
        }

        // ══════════════════════════════════════
        //  CHARGEMENT
        // ══════════════════════════════════════

        private async Task LoadData()
        {
            try
            {
                lblDateJour.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");

                // Dates par défaut : mois en cours
                dpDateFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dpDateTo.SelectedDate = DateTime.Now;

                // Charger les vendeurs pour le filtre
                var users = await App.Api.GetUsersAsync();
                var vendeurList = new List<UserDto>
                {
                    new UserDto { UserId = 0, Username = "Tous les vendeurs" }
                };
                vendeurList.AddRange(users);
                cboVendeur.ItemsSource = vendeurList;
                cboVendeur.SelectedIndex = 0;

                // Paiement par défaut
                cboPayment.SelectedIndex = 0;

                // Charger les ventes
                await LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadSales()
        {
            try
            {
                _allSales = await App.Api.GetSalesAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        //  FILTRES
        // ══════════════════════════════════════

        private void ApplyFilters()
        {
            _filteredSales = _allSales.ToList();

            // Filtre date début
            if (dpDateFrom.SelectedDate.HasValue)
            {
                var from = dpDateFrom.SelectedDate.Value.Date;
                _filteredSales = _filteredSales
                    .Where(s => s.SaleDate.Date >= from)
                    .ToList();
            }

            // Filtre date fin
            if (dpDateTo.SelectedDate.HasValue)
            {
                var to = dpDateTo.SelectedDate.Value.Date.AddDays(1);
                _filteredSales = _filteredSales
                    .Where(s => s.SaleDate.Date < to)
                    .ToList();
            }

            // Filtre vendeur
            if (cboVendeur.SelectedValue is int vendeurId && vendeurId > 0)
            {
                var vendeurName = (cboVendeur.SelectedItem as UserDto)?.Username ?? "";
                _filteredSales = _filteredSales
                    .Where(s => s.Vendeur == vendeurName)
                    .ToList();
            }

            // Filtre mode paiement
            if (cboPayment.SelectedItem is ComboBoxItem payItem)
            {
                var payment = payItem.Content?.ToString();
                if (!string.IsNullOrEmpty(payment) && payment != "Tous")
                {
                    _filteredSales = _filteredSales
                        .Where(s => s.PaymentMethod == payment)
                        .ToList();
                }
            }

            // Filtre N° facture
            var searchText = txtSearchInvoice.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                _filteredSales = _filteredSales
                    .Where(s => s.InvoiceNumber.ToLower().Contains(searchText))
                    .ToList();
            }

            // Appliquer au DataGrid
            dgSales.ItemsSource = _filteredSales;
            lblSaleCount.Text = $"• {_filteredSales.Count} vente(s)";

            // Mettre à jour le résumé
            UpdateSummary();

            // Réinitialiser le détail
            ClearDetail();
        }

        private void UpdateSummary()
        {
            var total = _filteredSales.Sum(s => s.TotalAmount);
            var count = _filteredSales.Count;
            var especes = _filteredSales
                .Where(s => s.PaymentMethod == "Espèces")
                .Sum(s => s.TotalAmount);
            var carte = _filteredSales
                .Where(s => s.PaymentMethod == "Carte")
                .Sum(s => s.TotalAmount);

            lblTotalVentes.Text = count.ToString();
            lblChiffreAffaires.Text = $"{total:N2} DA";
            lblTotalEspeces.Text = $"{especes:N2} DA";
            lblTotalCarte.Text = $"{carte:N2} DA";
        }

        // ══════════════════════════════════════
        //  ÉVÉNEMENTS FILTRES
        // ══════════════════════════════════════

        private void Filter_Changed(object sender, EventArgs e)
        {
            // Éviter les erreurs pendant l'initialisation
            if (!IsLoaded) return;
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            ApplyFilters();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            dpDateFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpDateTo.SelectedDate = DateTime.Now;
            cboVendeur.SelectedIndex = 0;
            cboPayment.SelectedIndex = 0;
            txtSearchInvoice.Text = "";
            ApplyFilters();
        }

        // ══════════════════════════════════════
        //  SÉLECTION D'UNE VENTE → DÉTAIL
        // ══════════════════════════════════════

        private void DgSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSales.SelectedItem is SaleDto sale)
            {
                ShowDetail(sale);
            }
        }

        private void ShowDetail(SaleDto sale)
        {
            // Header
            lblDetailTitle.Text = $"Facture {sale.InvoiceNumber}";

            // Infos
            lblDetailInvoice.Text = $"N° : {sale.InvoiceNumber}";
            lblDetailDate.Text = $"Date : {sale.SaleDate:dd/MM/yyyy à HH:mm}";
            lblDetailVendeur.Text = $"Vendeur : {sale.Vendeur}";
            lblDetailPayment.Text = $"Paiement : {sale.PaymentMethod}";

            // Lignes
            dgDetails.ItemsSource = sale.Details;

            // Total
            lblDetailTotal.Text = $"{sale.TotalAmount:N2} DA";
        }

        private void ClearDetail()
        {
            lblDetailTitle.Text = "Détail de la facture";
            lblDetailInvoice.Text = "N° : —";
            lblDetailDate.Text = "Date : —";
            lblDetailVendeur.Text = "Vendeur : —";
            lblDetailPayment.Text = "Paiement : —";
            dgDetails.ItemsSource = null;
            lblDetailTotal.Text = "0.00 DA";
        }
    }
}