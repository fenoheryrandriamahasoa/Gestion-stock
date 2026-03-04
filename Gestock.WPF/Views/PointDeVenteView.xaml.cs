using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class PointDeVenteView : UserControl
    {
        private List<ProductDto> _allProducts = new();
        private ObservableCollection<SaleLineItem> _cart = new();
        private ProductDto? _selectedProduct = null;

        public PointDeVenteView()
        {
            InitializeComponent();
            dgCart.ItemsSource = _cart;
            _cart.CollectionChanged += (s, e) => UpdateCartUI();

            Loaded += async (s, e) =>
            {
                lblTotal.Text = App.FormatPrice(0);   // ✅
                await LoadData();
            };
        }

        // ══════════════════════════════════════
        //  CHARGEMENT
        // ══════════════════════════════════════

        private async Task LoadData()
        {
            try
            {
                // Afficher le vendeur
                if (App.CurrentUser != null)
                {
                    lblVendeur.Text = $"{App.CurrentUser.RoleDisplay} : {App.CurrentUser.Username}";
                }

                // Charger tous les produits
                _allProducts = await App.Api.GetProductsAsync();

                // Afficher tous les produits au départ
                lstProducts.ItemsSource = _allProducts;
                lblResultCount.Text = $"{_allProducts.Count} produit(s)";
                lblNoResults.Visibility = Visibility.Collapsed;

                // Focus sur la recherche
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════
        //  RECHERCHE EN TEMPS RÉEL
        // ══════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                // Afficher tous les produits
                lstProducts.ItemsSource = _allProducts;
                lblResultCount.Text = $"{_allProducts.Count} produit(s)";
                lblNoResults.Visibility = _allProducts.Count == 0
                    ? Visibility.Visible : Visibility.Collapsed;
                return;
            }

            // Filtrer par nom OU code-barres OU catégorie
            var results = _allProducts.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Barcode.ToLower().Contains(search) ||
                p.CategoryName.ToLower().Contains(search)
            ).ToList();

            lstProducts.ItemsSource = results;
            lblResultCount.Text = $"{results.Count} résultat(s)";

            // Message si aucun résultat
            if (results.Count == 0)
            {
                lblNoResults.Text = "Aucun produit trouvé.";
                lblNoResults.Visibility = Visibility.Visible;
            }
            else
            {
                lblNoResults.Visibility = Visibility.Collapsed;
            }
        }

        // Entrée dans la recherche → ajouter directement si 1 seul résultat
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var search = txtSearch.Text.Trim().ToLower();

                if (string.IsNullOrEmpty(search)) return;

                // Chercher une correspondance exacte par code-barres
                var exactMatch = _allProducts.FirstOrDefault(p =>
                    p.Barcode.ToLower() == search);

                if (exactMatch != null)
                {
                    // Code-barres exact → ajouter directement au panier
                    AddProductToCart(exactMatch, 1);
                    txtSearch.Text = "";
                    txtSearch.Focus();
                    return;
                }

                // Si un seul résultat dans la liste → le sélectionner
                var currentResults = lstProducts.ItemsSource as List<ProductDto>;
                if (currentResults != null && currentResults.Count == 1)
                {
                    SelectProduct(currentResults[0]);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                txtSearch.Text = "";
                ClearSelection();
            }
        }

        // ══════════════════════════════════════
        //  SÉLECTION D'UN PRODUIT
        // ══════════════════════════════════════

        private void LstProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstProducts.SelectedItem is ProductDto product)
            {
                SelectProduct(product);
            }
        }

        private void SelectProduct(ProductDto product)
        {
            _selectedProduct = product;

            // Afficher les infos
            lblSelectedName.Text = product.Name;
            lblSelectedBarcode.Text = $"Code : {product.Barcode}";
            lblSelectedStock.Text = $"Stock disponible : {product.StockQuantity} {product.Unit}";
            lblSelectedPrice.Text = App.FormatPrice(product.SellingPrice);

            // Réinitialiser la quantité
            txtQty.Text = "1";

            // Afficher le panneau
            pnlProductDetail.Visibility = Visibility.Visible;
        }

        private void ClearSelection()
        {
            _selectedProduct = null;
            lstProducts.SelectedItem = null;
            pnlProductDetail.Visibility = Visibility.Collapsed;
        }

        // ══════════════════════════════════════
        //  GESTION QUANTITÉ
        // ══════════════════════════════════════

        private void BtnQtyMinus_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtQty.Text, out var qty) && qty > 1)
            {
                txtQty.Text = (qty - 1).ToString();
            }
        }

        private void BtnQtyPlus_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtQty.Text, out var qty))
            {
                // Vérifier le stock disponible
                if (_selectedProduct != null && qty + 1 > GetAvailableStock(_selectedProduct))
                {
                    MessageBox.Show(
                        $"Stock insuffisant !\nDisponible : {GetAvailableStock(_selectedProduct)}",
                        "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                txtQty.Text = (qty + 1).ToString();
            }
        }

        // Stock disponible = stock réel - quantité déjà dans le panier
        private int GetAvailableStock(ProductDto product)
        {
            var inCart = _cart
                .Where(c => c.ProductId == product.ProductId)
                .Sum(c => c.Quantity);
            return product.StockQuantity - inCart;
        }

        // ══════════════════════════════════════
        //  AJOUTER AU PANIER
        // ══════════════════════════════════════

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Sélectionnez un produit d'abord.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQty.Text, out var qty) || qty <= 0)
            {
                MessageBox.Show("Quantité invalide.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddProductToCart(_selectedProduct, qty);

            // Remettre le focus sur la recherche
            txtSearch.Text = "";
            txtSearch.Focus();
            ClearSelection();
        }

        private void AddProductToCart(ProductDto product, int quantity)
        {
            // Vérifier le stock
            var available = GetAvailableStock(product);
            if (quantity > available)
            {
                MessageBox.Show(
                    $"Stock insuffisant pour '{product.Name}' !\n" +
                    $"Disponible : {available}, Demandé : {quantity}",
                    "Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Si déjà dans le panier → incrémenter
            var existing = _cart.FirstOrDefault(c => c.ProductId == product.ProductId);

            if (existing != null)
            {
                existing.Quantity += quantity;

                // Forcer le rafraîchissement
                RefreshCart();
            }
            else
            {
                _cart.Add(new SaleLineItem
                {
                    LineNumber = _cart.Count + 1,
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    UnitPrice = product.SellingPrice,
                    Quantity = quantity
                });
            }

            UpdateCartUI();
        }

        // ══════════════════════════════════════
        //  SUPPRIMER UNE LIGNE DU PANIER
        // ══════════════════════════════════════

        private void BtnRemoveLine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SaleLineItem item)
            {
                _cart.Remove(item);
                RenumberCart();
                UpdateCartUI();
            }
        }

        // ══════════════════════════════════════
        //  MISE À JOUR UI PANIER
        // ══════════════════════════════════════

        private void UpdateCartUI()
        {
            var total = _cart.Sum(c => c.SubTotal);
            var count = _cart.Sum(c => c.Quantity);

            lblTotal.Text = App.FormatPrice(total);
            lblCartCount.Text = $"{count} article(s)";
            lblCartEmpty.Visibility = _cart.Count == 0
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RefreshCart()
        {
            var items = _cart.ToList();
            dgCart.ItemsSource = null;
            dgCart.ItemsSource = _cart;
        }

        private void RenumberCart()
        {
            for (int i = 0; i < _cart.Count; i++)
                _cart[i].LineNumber = i + 1;
        }

        // ══════════════════════════════════════
        //  VALIDER LA VENTE
        // ══════════════════════════════════════

        private async void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Le panier est vide !",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Erreur : aucun utilisateur connecté.");
                return;
            }

            var payment = (cboPayment.SelectedItem as ComboBoxItem)?
                .Content?.ToString() ?? "Espèces";

            var total = _cart.Sum(c => c.SubTotal);

            var result = MessageBox.Show(
                $"Confirmer la vente ?\n\n" +
                $"  📦 Articles : {_cart.Count} ligne(s)\n" +
                $"  💰 Total : {App.FormatPrice(total)}\n" +
                $"  💳 Paiement : {payment}\n",
                "Confirmer la vente",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

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

                MessageBox.Show(
                    $"✅ Vente enregistrée !\n\nTotal : {App.FormatPrice(total)}",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Vider et rafraîchir
                _cart.Clear();
                UpdateCartUI();
                ClearSelection();

                // Recharger les produits (stock mis à jour)
                _allProducts = await App.Api.GetProductsAsync();
                lstProducts.ItemsSource = _allProducts;
                txtSearch.Text = "";
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur vente", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════
        //  ANNULER / VIDER LE PANIER
        // ══════════════════════════════════════

        private void BtnCancelSale_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count > 0)
            {
                var result = MessageBox.Show(
                    "Vider tout le panier ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _cart.Clear();
                    UpdateCartUI();
                    ClearSelection();
                    txtSearch.Text = "";
                    txtSearch.Focus();
                }
            }
        }
    }
}