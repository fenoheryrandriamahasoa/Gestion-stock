using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Views
{
    public partial class DashboardView : UserControl
    {
        private List<SaleDto> _allSales = new();
        private List<ProductDto> _allProducts = new();
        private List<StockMovementDto> _allMovements = new();

        public DashboardView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadDashboard();
        }

        // ══════════════════════════════════════
        //  CHARGEMENT PRINCIPAL
        // ══════════════════════════════════════

        private async Task LoadDashboard()
        {
            try
            {
                lblDate.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");

                _allSales = await App.Api.GetSalesAsync();
                _allProducts = await App.Api.GetProductsAsync();
                _allMovements = await App.Api.GetStockMovementsAsync();

                LoadKPICards();
                LoadPaymentCards();
                LoadWeeklyBars();
                LoadTopProducts();
                LoadApproBars();
                LoadPaymentBars();
                LoadLastSales();
            }
            catch (Exception ex)
            {
                InitDefaults();
                pnlLastSales.Children.Clear();
                pnlLastSales.Children.Add(new TextBlock
                {
                    Text = $"Erreur : {ex.Message}",
                    Foreground = Brushes.Red,
                    Padding = new Thickness(10)
                });
            }
        }

        private void InitDefaults()
        {
            lblDashVentes.Text = App.FormatPrice(0);
            lblDashNbVentes.Text = "0 transaction(s)";
            lblDashArticles.Text = "0";
            lblDashTotalStock.Text = "0 unités totales";
            lblDashAlertes.Text = "0";
            lblDashRupture.Text = "0 en rupture";
            lblDashAppro.Text = "0";
            lblDashApproQty.Text = "0 unités ajoutées";
            lblPayEspeces.Text = App.FormatPrice(0);
            lblPayCarte.Text = App.FormatPrice(0);
            lblPayCheque.Text = App.FormatPrice(0);
            lblPayMobile.Text = App.FormatPrice(0);
            lblPayCount.Text = "0";
        }

        // ══════════════════════════════════════
        //  1. CARTES KPI
        // ══════════════════════════════════════

        private void LoadKPICards()
        {
            var today = DateTime.Today;
            var todaySales = _allSales.Where(s => s.SaleDate.Date == today).ToList();
            var thisMonth = _allMovements
                .Where(m => m.MovementDate.Month == today.Month &&
                            m.MovementDate.Year == today.Year)
                .ToList();

            lblDashVentes.Text = App.FormatPrice(todaySales.Sum(s => s.TotalAmount));
            lblDashNbVentes.Text = $"{todaySales.Count} transaction(s)";

            lblDashArticles.Text = _allProducts.Count.ToString();
            lblDashTotalStock.Text = $"{_allProducts.Sum(p => p.StockQuantity)} unités totales";

            var alertes = _allProducts.Where(p => p.IsLowStock).ToList();
            var ruptures = _allProducts.Where(p => p.StockQuantity == 0).ToList();
            lblDashAlertes.Text = alertes.Count.ToString();
            lblDashRupture.Text = $"{ruptures.Count} en rupture";

            lblDashAppro.Text = thisMonth.Count.ToString();
            lblDashApproQty.Text = $"{thisMonth.Sum(m => m.Quantity)} unités ajoutées";
        }

        // ══════════════════════════════════════
        //  2. CARTES PAIEMENTS
        // ══════════════════════════════════════

        private void LoadPaymentCards()
        {
            var todaySales = _allSales.Where(s => s.SaleDate.Date == DateTime.Today).ToList();

            lblPayEspeces.Text = App.FormatPrice(
                todaySales.Where(s => s.PaymentMethod == "Espèces").Sum(s => s.TotalAmount));
            lblPayCarte.Text = App.FormatPrice(
                todaySales.Where(s => s.PaymentMethod == "Carte").Sum(s => s.TotalAmount));
            lblPayCheque.Text = App.FormatPrice(
                todaySales.Where(s => s.PaymentMethod == "Chèque").Sum(s => s.TotalAmount));
            lblPayMobile.Text = App.FormatPrice(
                todaySales.Where(s => s.PaymentMethod == "MVola" ||
                                      s.PaymentMethod == "Orange Money" ||
                                      s.PaymentMethod == "Airtel Money")
                           .Sum(s => s.TotalAmount));
            lblPayCount.Text = todaySales.Count.ToString();
        }

        // ══════════════════════════════════════
        //  3. BARRES : Ventes 7 derniers jours
        // ══════════════════════════════════════

        private void LoadWeeklyBars()
        {
            pnlWeeklyBars.Children.Clear();

            var today = DateTime.Today;
            var dailyValues = new List<(string Label, double Value)>();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var total = (double)_allSales
                    .Where(s => s.SaleDate.Date == date)
                    .Sum(s => s.TotalAmount);
                dailyValues.Add((date.ToString("ddd\ndd/MM"), total));
            }

            var maxValue = dailyValues.Max(d => d.Value);
            if (maxValue == 0) maxValue = 1;

            foreach (var day in dailyValues)
            {
                var barHeight = (day.Value / maxValue) * 140;
                if (barHeight < 3 && day.Value > 0) barHeight = 3;

                var bar = new StackPanel
                {
                    Width = 50,
                    Margin = new Thickness(4, 0, 4, 0),
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                // Valeur au-dessus
                bar.Children.Add(new TextBlock
                {
                    Text = day.Value > 0 ? $"{day.Value:N0}" : "",
                    FontSize = 9,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 2)
                });

                // Barre
                bar.Children.Add(new Border
                {
                    Height = barHeight,
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#27AE60")),
                    CornerRadius = new CornerRadius(3, 3, 0, 0)
                });

                // Label en-dessous
                bar.Children.Add(new TextBlock
                {
                    Text = day.Label,
                    FontSize = 9,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                });

                pnlWeeklyBars.Children.Add(bar);
            }
        }

        // ══════════════════════════════════════
        //  4. BARRES HORIZONTALES : Top 5 produits
        // ══════════════════════════════════════

        private void LoadTopProducts()
        {
            pnlTopProducts.Children.Clear();

            var today = DateTime.Today;
            var monthSales = _allSales
                .Where(s => s.SaleDate.Month == today.Month &&
                            s.SaleDate.Year == today.Year)
                .ToList();

            var productTotals = monthSales
                .SelectMany(s => s.Details)
                .GroupBy(d => d.ProductName)
                .Select(g => new { Name = g.Key, Total = g.Sum(d => d.Quantity) })
                .OrderByDescending(p => p.Total)
                .Take(5)
                .ToList();

            if (productTotals.Count == 0)
            {
                pnlTopProducts.Children.Add(new TextBlock
                {
                    Text = "Aucune vente ce mois",
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#BDC3C7")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            var maxQty = productTotals.Max(p => p.Total);
            if (maxQty == 0) maxQty = 1;

            var colors = new[] { "#27AE60", "#2980B9", "#E67E22", "#8E44AD", "#E74C3C" };
            int colorIndex = 0;

            foreach (var product in productTotals)
            {
                var barWidth = ((double)product.Total / maxQty) * 200;
                if (barWidth < 5) barWidth = 5;

                var row = new Grid { Margin = new Thickness(0, 3, 0, 3) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });

                // Nom
                var name = new TextBlock
                {
                    Text = product.Name.Length > 15
                        ? product.Name.Substring(0, 15) + "..."
                        : product.Name,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#34495E")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(name, 0);
                row.Children.Add(name);

                // Barre
                var bar = new Border
                {
                    Width = barWidth,
                    Height = 18,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    CornerRadius = new CornerRadius(0, 5, 5, 0),
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(colors[colorIndex % colors.Length]))
                };
                Grid.SetColumn(bar, 1);
                row.Children.Add(bar);

                // Quantité
                var qty = new TextBlock
                {
                    Text = product.Total.ToString(),
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#34495E")),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                Grid.SetColumn(qty, 2);
                row.Children.Add(qty);

                pnlTopProducts.Children.Add(row);
                colorIndex++;
            }
        }

        // ══════════════════════════════════════
        //  5. BARRES : Approvisionnements par semaine
        // ══════════════════════════════════════

        private void LoadApproBars()
        {
            pnlApproBars.Children.Clear();

            var today = DateTime.Today;
            var weeks = new[] { "Sem.1", "Sem.2", "Sem.3", "Sem.4", "Sem.5" };
            var weekValues = new double[5];

            var monthMovements = _allMovements
                .Where(m => m.MovementDate.Month == today.Month &&
                            m.MovementDate.Year == today.Year)
                .ToList();

            foreach (var mvt in monthMovements)
            {
                int weekIndex = Math.Min((mvt.MovementDate.Day - 1) / 7, 4);
                weekValues[weekIndex] += mvt.Quantity;
            }

            int currentWeek = Math.Min((today.Day - 1) / 7, 4);
            var maxValue = weekValues.Take(currentWeek + 1).Max();
            if (maxValue == 0) maxValue = 1;

            for (int i = 0; i <= currentWeek; i++)
            {
                var barHeight = (weekValues[i] / maxValue) * 140;
                if (barHeight < 3 && weekValues[i] > 0) barHeight = 3;

                var bar = new StackPanel
                {
                    Width = 60,
                    Margin = new Thickness(6, 0, 6, 0),
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                bar.Children.Add(new TextBlock
                {
                    Text = weekValues[i] > 0 ? $"{weekValues[i]:N0}" : "",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 2)
                });

                bar.Children.Add(new Border
                {
                    Height = barHeight,
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#8E44AD")),
                    CornerRadius = new CornerRadius(3, 3, 0, 0)
                });

                bar.Children.Add(new TextBlock
                {
                    Text = weeks[i],
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                });

                pnlApproBars.Children.Add(bar);
            }
        }

        // ══════════════════════════════════════
        //  6. BARRES HORIZONTALES : Répartition paiements mois
        // ══════════════════════════════════════

        private void LoadPaymentBars()
        {
            pnlPaymentBars.Children.Clear();

            var today = DateTime.Today;
            var monthSales = _allSales
                .Where(s => s.SaleDate.Month == today.Month &&
                            s.SaleDate.Year == today.Year)
                .ToList();

            var payments = new[]
            {
                ("💵 Espèces", monthSales.Where(s => s.PaymentMethod == "Espèces").Sum(s => s.TotalAmount), "#27AE60"),
                ("💳 Carte", monthSales.Where(s => s.PaymentMethod == "Carte").Sum(s => s.TotalAmount), "#2980B9"),
                ("📝 Chèque", monthSales.Where(s => s.PaymentMethod == "Chèque").Sum(s => s.TotalAmount), "#E67E22"),
                ("📱 Mobile", monthSales.Where(s =>
                    s.PaymentMethod == "MVola" ||
                    s.PaymentMethod == "Orange Money" ||
                    s.PaymentMethod == "Airtel Money").Sum(s => s.TotalAmount), "#16A085")
            };

            var totalMonth = payments.Sum(p => p.Item2);
            if (totalMonth == 0)
            {
                pnlPaymentBars.Children.Add(new TextBlock
                {
                    Text = "Aucune vente ce mois",
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#BDC3C7")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var (label, amount, color) in payments)
            {
                if (amount == 0) continue;

                var percent = (double)(amount / totalMonth) * 100;
                var barWidth = (double)(amount / totalMonth) * 150;
                if (barWidth < 5) barWidth = 5;

                var row = new Grid { Margin = new Thickness(0, 5, 0, 5) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });

                var nameBlock = new TextBlock
                {
                    Text = label,
                    FontSize = 11,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#34495E")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(nameBlock, 0);
                row.Children.Add(nameBlock);

                var bar = new Border
                {
                    Width = barWidth,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    CornerRadius = new CornerRadius(0, 5, 5, 0),
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(color))
                };
                Grid.SetColumn(bar, 1);
                row.Children.Add(bar);

                var valueBlock = new TextBlock
                {
                    Text = $"{percent:N0}% ({App.FormatPrice(amount)})",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                Grid.SetColumn(valueBlock, 2);
                row.Children.Add(valueBlock);

                pnlPaymentBars.Children.Add(row);
            }
        }

        // ══════════════════════════════════════
        //  7. DERNIÈRES FACTURES
        // ══════════════════════════════════════

        private void LoadLastSales()
        {
            var lastSales = _allSales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToList();

            pnlLastSales.Children.Clear();

            if (lastSales.Count == 0)
            {
                pnlLastSales.Children.Add(new TextBlock
                {
                    Text = "Aucune vente enregistrée.",
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#BDC3C7")),
                    Padding = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                return;
            }

            foreach (var sale in lastSales)
            {
                var line = new TextBlock
                {
                    Padding = new Thickness(5),
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#2C3E50"))
                };

                line.Inlines.Add(new Run($"• {sale.InvoiceNumber}")
                { FontWeight = FontWeights.SemiBold });

                line.Inlines.Add(new Run(
                    $"  —  {sale.SaleDate:dd/MM/yyyy HH:mm}  —  {sale.Vendeur}  —  ")
                {
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#7F8C8D"))
                });

                line.Inlines.Add(new Run(App.FormatPrice(sale.TotalAmount))
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#27AE60"))
                });

                line.Inlines.Add(new Run($"  ({sale.PaymentMethod})")
                {
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#95A5A6")),
                    FontSize = 11
                });

                pnlLastSales.Children.Add(line);

                if (sale != lastSales.Last())
                {
                    pnlLastSales.Children.Add(new Separator
                    {
                        Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F1F2F6")),
                        Margin = new Thickness(0, 2, 0, 2)
                    });
                }
            }
        }
    }
}