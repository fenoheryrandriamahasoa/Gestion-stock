using FontAwesome.Sharp;
using SuperMarcheApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using FontAwesome.Sharp;

namespace SuperMarcheApp.Views
{
    public partial class CategoriesView : UserControl
    {
        private List<CategoryDto> _allCategories = new();
        private int? _editingId = null;

        public CategoriesView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadCategories();
        }

        // ══════════════════════════════════════
        //  CHARGEMENT
        // ══════════════════════════════════════

        private async Task LoadCategories()
        {
            try
            {
                _allCategories = await App.Api.GetCategoriesAsync();
                ApplyFilter();
                lblCategoryCount.Text = $"{_allCategories.Count} catégorie(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════
        //  RECHERCHE
        // ══════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var search = txtSearch.Text.Trim().ToLower();

            List<CategoryDto> filtered;

            if (string.IsNullOrEmpty(search))
            {
                filtered = _allCategories;
            }
            else
            {
                filtered = _allCategories
                    .Where(c => c.Name.ToLower().Contains(search) ||
                                c.Description.ToLower().Contains(search))
                    .ToList();
            }

            dgCategories.ItemsSource = filtered;
            lblFilterCount.Text = $"• {filtered.Count} résultat(s)";

            // Message si vide
            lblEmpty.Visibility = filtered.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // ══════════════════════════════════════
        //  SÉLECTION DANS LE DATAGRID
        // ══════════════════════════════════════

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ne rien faire ici (on utilise les boutons Edit/Delete)
        }

        // ══════════════════════════════════════
        //  BOUTON ÉDITER (dans la ligne)
        // ══════════════════════════════════════

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CategoryDto cat)
            {
                _editingId = cat.CategoryId;
                txtCategoryName.Text = cat.Name;
                txtCategoryDescription.Text = cat.Description;

                // Changer l'apparence du formulaire
                lblFormTitle.Text = "Modifier la Catégorie";
                lblFormSubtitle.Text = $"Modification de « {cat.Name} »";
                btnSaveCategory.Content = "💾 Modifier";
                btnSaveCategory.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2980B9"));
                icnForm.Icon = IconChar.Pencil;
                brdFormIcon.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EBF5FB"));

                // Focus sur le nom
                txtCategoryName.Focus();
                txtCategoryName.SelectAll();
            }
        }

        // ══════════════════════════════════════
        //  BOUTON SUPPRIMER (dans la ligne)
        // ══════════════════════════════════════

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CategoryDto cat)
            {
                // Vérifier si des produits utilisent cette catégorie
                if (cat.ProductCount > 0)
                {
                    MessageBox.Show(
                        $"Impossible de supprimer « {cat.Name} » !\n\n" +
                        $"{cat.ProductCount} produit(s) utilisent cette catégorie.\n" +
                        $"Déplacez d'abord ces produits vers une autre catégorie.",
                        "Suppression impossible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Supprimer la catégorie « {cat.Name} » ?\n\nCette action est irréversible.",
                    "Confirmer la suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.Api.DeleteCategoryAsync(cat.CategoryId);

                        MessageBox.Show($"✅ Catégorie « {cat.Name} » supprimée.",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                        ClearForm();
                        await LoadCategories();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // ══════════════════════════════════════
        //  ENREGISTRER / MODIFIER
        // ══════════════════════════════════════

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var name = txtCategoryName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Le nom de la catégorie est obligatoire.",
                    "Champ requis", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCategoryName.Focus();
                return;
            }

            // Vérifier si le nom existe déjà (sauf pour la catégorie en cours d'édition)
            var duplicate = _allCategories
                .FirstOrDefault(c => c.Name.ToLower() == name.ToLower() &&
                                     c.CategoryId != (_editingId ?? 0));

            if (duplicate != null)
            {
                MessageBox.Show($"Une catégorie « {duplicate.Name} » existe déjà.",
                    "Doublon", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var request = new CategoryRequest
                {
                    Name = name,
                    Description = txtCategoryDescription.Text.Trim()
                };

                if (_editingId.HasValue)
                {
                    // Modification
                    request.CategoryId = _editingId.Value;
                    await App.Api.PutCategoryAsync(_editingId.Value, request);

                    MessageBox.Show($"✅ Catégorie « {name} » modifiée avec succès !",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Création
                    await App.Api.PostCategoryAsync(request);

                    MessageBox.Show($"✅ Catégorie « {name} » créée avec succès !",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ClearForm();
                await LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════
        //  ANNULER
        // ══════════════════════════════════════

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // ══════════════════════════════════════
        //  RÉINITIALISER LE FORMULAIRE
        // ══════════════════════════════════════

        private void ClearForm()
        {
            _editingId = null;
            txtCategoryName.Text = "";
            txtCategoryDescription.Text = "";
            dgCategories.SelectedItem = null;

            // Remettre le formulaire en mode "création"
            lblFormTitle.Text = "Nouvelle Catégorie";
            lblFormSubtitle.Text = "Remplissez les informations ci-dessous";
            btnSaveCategory.Content = "💾 Enregistrer";
            btnSaveCategory.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#27AE60"));
            icnForm.Icon = IconChar.Plus;
            brdFormIcon.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EBF5FB"));
        }
    }
}