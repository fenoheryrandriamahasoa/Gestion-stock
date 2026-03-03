using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class CategoriesView : UserControl
    {
        private List<CategoryDto> _categories = new();
        private int? _editingId = null;  // null = création, sinon = modification

        public CategoriesView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadCategories();
        }

        // ── Charger les catégories ──
        private async Task LoadCategories()
        {
            try
            {
                _categories = await App.Api.GetCategoriesAsync();
                dgCategories.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Sélection dans le DataGrid → remplir le formulaire ──
        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCategories.SelectedItem is CategoryDto cat)
            {
                _editingId = cat.CategoryId;
                txtCategoryName.Text = cat.Name;
                txtCategoryDescription.Text = cat.Description;
                btnSaveCategory.Content = "Modifier";
            }
        }

        // ── Enregistrer / Modifier ──
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var name = txtCategoryName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Le nom est obligatoire.",
                    "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    // ✅ Modification
                    request.CategoryId = _editingId.Value;
                    await App.Api.PutCategoryAsync(_editingId.Value, request);
                    MessageBox.Show("Catégorie modifiée !", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // ✅ Création
                    await App.Api.PostCategoryAsync(request);
                    MessageBox.Show("Catégorie ajoutée !", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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

        // ── Supprimer ──
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem is not CategoryDto cat)
            {
                MessageBox.Show("Sélectionnez une catégorie à supprimer.");
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer la catégorie '{cat.Name}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await App.Api.DeleteCategoryAsync(cat.CategoryId);
                    MessageBox.Show("Catégorie supprimée !", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ClearForm()
        {
            _editingId = null;
            txtCategoryName.Text = "";
            txtCategoryDescription.Text = "";
            btnSaveCategory.Content = "Enregistrer";
            dgCategories.SelectedItem = null;
        }
    }
}