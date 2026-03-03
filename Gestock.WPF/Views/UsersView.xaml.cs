using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class UsersView : UserControl
    {
        private List<UserDto> _allUsers = new();
        private int? _editingId = null;

        public UsersView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                _allUsers = await App.Api.GetUsersAsync();
                dgUsers.ItemsSource = _allUsers;
                lblUserCount.Text = $"• {_allUsers.Count} Utilisateur(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        // ── Filtre recherche ──
        private void TxtFiltre_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = txtFiltreUser.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                dgUsers.ItemsSource = _allUsers;
            }
            else
            {
                var filtered = _allUsers
                    .Where(u => u.Username.ToLower().Contains(search))
                    .ToList();
                dgUsers.ItemsSource = filtered;
            }
        }

        // ── Éditer (bouton dans la ligne) ──
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UserDto user)
            {
                _editingId = user.UserId;
                txtNomUser.Text = user.Username;
                txtPassword.Password = ""; // Ne pas afficher le hash

                // Sélectionner le rôle
                foreach (ComboBoxItem item in cboRole.Items)
                {
                    if (item.Content.ToString() == user.Role)
                    {
                        cboRole.SelectedItem = item;
                        break;
                    }
                }

                ckbxActive.IsChecked = user.IsActive;
            }
        }

        // ── Supprimer (bouton dans la ligne) ──
        private async void BtnRowDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UserDto user)
            {
                if (user.UserId == App.CurrentUser?.UserId)
                {
                    MessageBox.Show("Vous ne pouvez pas supprimer votre propre compte !");
                    return;
                }

                var result = MessageBox.Show(
                    $"Supprimer l'utilisateur '{user.Username}' ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.Api.DeleteUserAsync(user.UserId);
                        MessageBox.Show("Utilisateur supprimé !");
                        ClearForm();
                        await LoadUsers();
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
            var username = txtNomUser.Text.Trim();
            var password = txtPassword.Password;
            var role = (cboRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Seller";
            var isActive = ckbxActive.IsChecked ?? true;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Le nom d'utilisateur est obligatoire.");
                return;
            }

            // Mot de passe obligatoire pour la création
            if (!_editingId.HasValue && string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Le mot de passe est obligatoire pour un nouvel utilisateur.");
                return;
            }

            try
            {
                var request = new UserRequest
                {
                    Username = username,
                    PasswordHash = password,  // L'API le hashera
                    Role = role,
                    IsActive = isActive
                };

                if (_editingId.HasValue)
                {
                    request.UserId = _editingId.Value;

                    // Si mot de passe vide → on doit le gérer côté API
                    if (string.IsNullOrEmpty(password))
                    {
                        request.PasswordHash = "";  // L'API gardera l'ancien
                    }

                    await App.Api.PutUserAsync(_editingId.Value, request);
                    MessageBox.Show("Utilisateur modifié !");
                }
                else
                {
                    await App.Api.PostUserAsync(request);
                    MessageBox.Show("Utilisateur créé !");
                }

                ClearForm();
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _editingId = null;
            txtNomUser.Text = "";
            txtPassword.Password = "";
            cboRole.SelectedIndex = 0;
            ckbxActive.IsChecked = true;
            dgUsers.SelectedItem = null;
        }
    }
}