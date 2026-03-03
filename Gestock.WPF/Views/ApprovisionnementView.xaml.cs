using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class ApprovisionnementView : UserControl
    {
        private List<StockMovementDto> _allMovements = new();

        public ApprovisionnementView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                // Charger les produits pour le ComboBox
                var products = await App.Api.GetProductsAsync();
                cboProduit.ItemsSource = products;

                // Date par défaut
                dpDate.SelectedDate = DateTime.Today;

                // Charger les mouvements
                await LoadMovements();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private async Task LoadMovements()
        {
            _allMovements = await App.Api.GetStockMovementsAsync();
            dgMovements.ItemsSource = _allMovements;
            lblApproCount.Text = $"• {_allMovements.Count} Mouvement(s)";
        }

        // ── Filtre ──
        private void TxtFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = txtFiltreAppro.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                dgMovements.ItemsSource = _allMovements;
            }
            else
            {
                dgMovements.ItemsSource = _allMovements
                    .Where(m => m.ProductName.ToLower().Contains(search) ||
                                m.Notes.ToLower().Contains(search))
                    .ToList();
            }
        }

        // ── Valider l'entrée de stock ──
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cboProduit.SelectedValue == null)
            {
                MessageBox.Show("Sélectionnez un produit.");
                return;
            }

            if (!int.TryParse(txtQuantite.Text, out var qty) || qty <= 0)
            {
                MessageBox.Show("La quantité doit être un nombre positif.");
                return;
            }

            try
            {
                var request = new StockMovementRequest
                {
                    ProductId = (int)cboProduit.SelectedValue,
                    Quantity = qty,
                    Notes = txtNotes.Text.Trim()
                };

                await App.Api.PostStockMovementAsync(request);
                MessageBox.Show("Entrée de stock validée !", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
                await LoadMovements();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        // ── Supprimer un mouvement ──
        private async void BtnRowDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StockMovementDto mvt)
            {
                var result = MessageBox.Show(
                    $"Supprimer ce mouvement ({mvt.ProductName}, +{mvt.Quantity}) ?\nLe stock sera diminué.",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.Api.DeleteStockMovementAsync(mvt.MovementId);
                        await LoadMovements();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}");
                    }
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            cboProduit.SelectedIndex = -1;
            txtQuantite.Text = "1";
            txtNotes.Text = "";
            dpDate.SelectedDate = DateTime.Today;
        }
    }
}