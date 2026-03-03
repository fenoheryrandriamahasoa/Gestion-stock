using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class ArticlesView : UserControl
    {
        private List<ProductDto> _allProducts = new();
        private List<CategoryDto> _categories = new();
        private int? _editingId = null;

        public ArticlesView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadData();
        }

        // ── Chargement initial ──
        private async Task LoadData()
        {
            try
            {
                // Charger les catégories pour le ComboBox
                _categories = await App.Api.GetCategoriesAsync();

                cboCategorie.ItemsSource = _categories;

                // Filtre catégorie
                var filterList = new List<CategoryDto>
                {
                    new CategoryDto { CategoryId = 0, Name = "Toutes les catégories" }
                };
                filterList.AddRange(_categories);
                cboFiltreCategorie.ItemsSource = filterList;
                cboFiltreCategorie.SelectedIndex = 0;

                // Charger les produits
                await LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProducts()
        {
            _allProducts = await App.Api.GetProductsAsync();
            ApplyFilter();
        }

        // ── Filtre par catégorie ──
        private void CboFiltre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (cboFiltreCategorie.SelectedValue is int catId && catId > 0)
            {
                var filtered = _allProducts.Where(p => p.CategoryId == catId).ToList();
                dgArticles.ItemsSource = filtered;
                lblArticleCount.Text = $"• {filtered.Count} article(s)";
            }
            else
            {
                dgArticles.ItemsSource = _allProducts;
                lblArticleCount.Text = $"• {_allProducts.Count} article(s)";
            }
        }

        // ── Sélection ligne DataGrid ──
        private void DgArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optionnel : remplir le formulaire
        }

        // ── Bouton Éditer (dans la ligne) ──
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ProductDto product)
            {
                _editingId = product.ProductId;

                txtBarcode.Text = product.Barcode;
                txtNom.Text = product.Name;
                cboCategorie.SelectedValue = product.CategoryId;
                txtPrixAchat.Text = product.PurchasePrice.ToString("F2");
                txtPrixVente.Text = product.SellingPrice.ToString("F2");
                txtStock.Text = product.StockQuantity.ToString();
                txtStockMin.Text = product.MinStockAlert.ToString();

                // Sélectionner l'unité dans le ComboBox
                foreach (ComboBoxItem item in cboUnit.Items)
                {
                    if (item.Content.ToString() == product.Unit)
                    {
                        cboUnit.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // ── Bouton Supprimer (dans la ligne) ──
        private async void BtnRowDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ProductDto product)
            {
                var result = MessageBox.Show(
                    $"Supprimer '{product.Name}' ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.Api.DeleteProductAsync(product.ProductId);
                        MessageBox.Show("Produit supprimé !");
                        await LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}");
                    }
                }
            }
        }

        // ── Enregistrer / Modifier ──
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtBarcode.Text) ||
                string.IsNullOrWhiteSpace(txtNom.Text) ||
                cboCategorie.SelectedValue == null)
            {
                MessageBox.Show("Veuillez remplir les champs obligatoires.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrixAchat.Text, out var prixAchat) ||
                !decimal.TryParse(txtPrixVente.Text, out var prixVente) ||
                !int.TryParse(txtStock.Text, out var stock) ||
                !int.TryParse(txtStockMin.Text, out var stockMin))
            {
                MessageBox.Show("Vérifiez les valeurs numériques.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var unit = (cboUnit.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Pcs";

            try
            {
                var request = new ProductRequest
                {
                    Barcode = txtBarcode.Text.Trim(),
                    Name = txtNom.Text.Trim(),
                    CategoryId = (int)cboCategorie.SelectedValue,
                    PurchasePrice = prixAchat,
                    SellingPrice = prixVente,
                    StockQuantity = stock,
                    MinStockAlert = stockMin,
                    Unit = unit
                };

                if (_editingId.HasValue)
                {
                    request.ProductId = _editingId.Value;
                    await App.Api.PutProductAsync(_editingId.Value, request);
                    MessageBox.Show("Produit modifié !");
                }
                else
                {
                    await App.Api.PostProductAsync(request);
                    MessageBox.Show("Produit ajouté !");
                }

                ClearForm();
                await LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Annuler ──
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _editingId = null;
            txtBarcode.Text = "";
            txtNom.Text = "";
            txtPrixAchat.Text = "";
            txtPrixVente.Text = "";
            txtStock.Text = "";
            txtStockMin.Text = "";
            cboCategorie.SelectedIndex = -1;
            cboUnit.SelectedIndex = 0;
            dgArticles.SelectedItem = null;
        }
    }
}