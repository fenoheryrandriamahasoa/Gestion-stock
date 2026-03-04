using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SuperMarcheApp.Models;

namespace SuperMarcheApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        // CHANGEZ CE PORT selon votre launchSettings.json de l'API
        private const string BASE_URL = "http://localhost:5013";

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                // Accepter les certificats SSL dev (localhost)
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri(BASE_URL)
            };
        }

        // ══════════════════════════════════════
        //  MÉTHODES GÉNÉRIQUES
        // ══════════════════════════════════════

        public async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _http.GetAsync(endpoint);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return default;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            return await _http.PostAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            return await _http.PutAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _http.DeleteAsync(endpoint);
        }

        // ══════════════════════════════════════
        //  AUTH
        // ══════════════════════════════════════

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var loginDto = new LoginDto { Username = username, Password = password };
            var response = await _http.PostAsJsonAsync("api/Users/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }

            // Lire le message d'erreur de l'API
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        // ══════════════════════════════════════
        //  CATEGORIES
        // ══════════════════════════════════════

        public Task<List<CategoryDto>> GetCategoriesAsync()
            => GetListAsync<CategoryDto>("api/Categories");

        public async Task PostCategoryAsync(CategoryRequest category)
        {
            var response = await PostAsync("api/Categories", category);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutCategoryAsync(int id, CategoryRequest category)
        {
            var response = await PutAsync($"api/Categories/{id}", category);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var response = await DeleteAsync($"api/Categories/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        // ══════════════════════════════════════
        //  PRODUITS
        // ══════════════════════════════════════

        public Task<List<ProductDto>> GetProductsAsync()
            => GetListAsync<ProductDto>("api/Products");

        public Task<ProductDto?> GetProductByBarcodeAsync(string barcode)
            => GetAsync<ProductDto>($"api/Products/barcode/{barcode}");

        public async Task PostProductAsync(ProductRequest product)
        {
            var response = await PostAsync("api/Products", product);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task PutProductAsync(int id, ProductRequest product)
        {
            var response = await PutAsync($"api/Products/{id}", product);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await DeleteAsync($"api/Products/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ══════════════════════════════════════
        //  UTILISATEURS
        // ══════════════════════════════════════

        public Task<List<UserDto>> GetUsersAsync()
            => GetListAsync<UserDto>("api/Users");

        public async Task PostUserAsync(UserRequest user)
        {
            var response = await PostAsync("api/Users", user);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task PutUserAsync(int id, UserRequest user)
        {
            var response = await PutAsync($"api/Users/{id}", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserAsync(int id)
        {
            var response = await DeleteAsync($"api/Users/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ══════════════════════════════════════
        //  MOUVEMENTS DE STOCK
        // ══════════════════════════════════════

        public Task<List<StockMovementDto>> GetStockMovementsAsync()
            => GetListAsync<StockMovementDto>("api/StockMovements");

        public async Task PostStockMovementAsync(StockMovementRequest movement)
        {
            var response = await PostAsync("api/StockMovements", movement);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteStockMovementAsync(int id)
        {
            var response = await DeleteAsync($"api/StockMovements/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ══════════════════════════════════════
        //  VENTES
        // ══════════════════════════════════════

        public Task<List<SaleDto>> GetSalesAsync()
            => GetListAsync<SaleDto>("api/Sales");

        public async Task<SaleDto?> PostSaleAsync(SaleRequest sale)
        {
            var response = await PostAsync("api/Sales", sale);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<SaleDto>();
        }

        // ══════════════════════════════════════
        //  PARAMÈTRES
        // ══════════════════════════════════════

        public async Task<AppSettingsDto> GetSettingsAsync()
        {
            var result = await GetAsync<AppSettingsDto>("api/Settings");
            return result ?? new AppSettingsDto
            {
                Id = 1,
                StoreName = "SUPERMARCHÉ",
                CurrencySymbol = "DA",
                CurrencyAfterAmount = true,
                InvoicePrefix = "FAC",
                ReceiptFooter = "Merci pour votre achat !",
                DefaultMinStockAlert = 5
            };
        }

        public async Task SaveSettingsAsync(AppSettingsDto settings)
        {
            var response = await PutAsync("api/Settings", settings);
            response.EnsureSuccessStatusCode();
        }
    }
}