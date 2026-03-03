using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class PointDeVenteView : UserControl
    {
        // Panier local avec mise à jour auto du DataGrid
        private ObservableCollection<SaleLineItem> _cart = new();

        public PointDeVenteView()
        {
            InitializeComponent();
            dgCart.ItemsSource = _cart;

            // Mettre à jour le total quand le panier change
            _cart.CollectionChanged += (s, e) => UpdateTotal();
        }

        // ── Scanner / Rechercher (Entrée) ──
        private async void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var search = txtFiltreProduct.Text.Trim();
            if (string.IsNullOrEmpty(search)) return;

            try
            {
                // Chercher par code-barres
                var product = await App.Api.GetProductByBarcodeAsync(search);

                if (product == null)
                {
                    // Chercher par nom dans la liste complète
                    var allProducts = await App.Api.GetProductsAsync();
                    product = allProducts.FirstOrDefault(p =>
                        p.Name.ToLower().Contains(search.ToLower()));
                }

                if (product == null)
                {
                    MessageBox.Show("Produit non trouvé.",
                        "Recherche", MessageBoxButton.OK, MessageBoxImage.Information);
                    lblResultCount.Text = "• 0 Résultats";
                    return;
                }

                lblResultCount.Text = $"• Trouvé : {product.Name}";

                // Vérifier le stock
                if (product.StockQuantity <= 0)
                {
                    MessageBox.Show($"'{product.Name}' est en rupture de stock !",
                        "Stock épuisé", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Si déjà dans le panier → incrémenter
                var existing = _cart.FirstOrDefault(c => c.ProductId == product.ProductId);

                if (existing != null)
                {
                    if (existing.Quantity + 1 > product.StockQuantity)
                    {
                        MessageBox.Show($"Stock insuffisant ! Disponible : {product.StockQuantity}");
                        return;
                    }

                    existing.Quantity++;

                    // Forcer le rafraîchissement du DataGrid
                    dgCart.ItemsSource = null;
                    dgCart.ItemsSource = _cart;
                }
                else
                {
                    // Ajouter au panier
                    _cart.Add(new SaleLineItem
                    {
                        LineNumber = _cart.Count + 1,
                        ProductId = product.ProductId,
                        ProductName = product.Name,
                        UnitPrice = product.SellingPrice,
                        Quantity = 1
                    });
                }

                UpdateTotal();
                txtFiltreProduct.Text = "";
                txtFiltreProduct.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        // ── Supprimer une ligne du panier ──
        private void BtnRemoveLine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SaleLineItem item)
            {
                _cart.Remove(item);

                // Renuméroter
                for (int i = 0; i < _cart.Count; i++)
                    _cart[i].LineNumber = i + 1;

                dgCart.ItemsSource = null;
                dgCart.ItemsSource = _cart;
                UpdateTotal();
            }
        }

        // ── Mettre à jour le total ──
        private void UpdateTotal()
        {
            var total = _cart.Sum(c => c.SubTotal);
            lblTotal.Text = $"{total:N2} DA";
        }

        // ── Valider la vente ──
        private async void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Le panier est vide !");
                return;
            }

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Erreur : aucun utilisateur connecté.");
                return;
            }

            var payment = (cboPayment.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Espèces";

            var result = MessageBox.Show(
                $"Confirmer la vente ?\n\n" +
                $"Articles : {_cart.Count}\n" +
                $"Total : {_cart.Sum(c => c.SubTotal):N2} DA\n" +
                $"Paiement : {payment}",
                "Confirmer", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var saleRequest = new SaleRequest
                {
                    UserId = App.CurrentUser.UserId,
                    PaymentMethod = payment,
                    SaleDetails = _cart.Select(c => new SaleDetailRequest
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity
                    }).ToList()
                };

                await App.Api.PostSaleAsync(saleRequest);

                MessageBox.Show("✅ Vente enregistrée avec succès !",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Vider le panier
                _cart.Clear();
                UpdateTotal();
                lblResultCount.Text = "• 0 Résultats";
                txtFiltreProduct.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur vente", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Annuler tout ──
        private void BtnCancelSale_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count > 0)
            {
                var result = MessageBox.Show("Vider le panier ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _cart.Clear();
                    UpdateTotal();
                }
            }
        }
    }
}