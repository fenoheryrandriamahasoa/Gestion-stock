using System.Windows;
using SuperMarcheApp.Models;
using SuperMarcheApp.Services;

namespace SuperMarcheApp
{
    public partial class App : Application
    {
        // Service API partagé par toute l'app
        public static ApiService Api { get; } = new ApiService();

        // Utilisateur connecté (accessible partout)
        public static UserDto? CurrentUser { get; set; }

        // Paramètres globaux (accessibles partout)
        public static AppSettingsDto Settings { get; set; } = new AppSettingsDto
        {
            StoreName = "SUPERMARCHÉ",
            CurrencySymbol = "DA",
            CurrencyAfterAmount = true,
            InvoicePrefix = "FAC",
            DefaultMinStockAlert = 5
        };

        // Raccourci pour formater les prix
        public static string FormatPrice(decimal amount)
        {
            return Settings.FormatPrice(amount);
        }
    }
}