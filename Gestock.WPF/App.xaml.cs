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
    }
}