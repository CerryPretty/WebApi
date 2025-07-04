using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebUI.Models; // Ensure this points to your WebUI's Product model
using Microsoft.Extensions.Configuration; // Required for configuration

namespace WebUI.Services
{
    public class ProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl; // No longer hardcoded

        // Constructor now accepts IConfiguration to read settings
        public ProductApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Get the base URL from appsettings.json
            _baseApiUrl = configuration.GetValue<string>("ApiSettings:ProductApiUrl") ?? "http://localhost:5000/api/products/";

            // Optional: Set a BaseAddress for the HttpClient if not already set by AddHttpClient
            // This can prevent needing to prefix _baseApiUrl in each method if it's purely a base URL
            // If your _baseApiUrl is already specific like "http://localhost:5000/api/products/",
            // then adding it to each request might be clearer.
            // If _baseApiUrl was just "http://localhost:5000/", then you'd set BaseAddress.
            // For now, we'll keep the current usage pattern.
        }

        // --- Common API Call Pattern Refactoring ---
        // Let's make error handling a bit more explicit

        public async Task<List<Product>?> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseApiUrl);
                response.EnsureSuccessStatusCode(); // Throws on 4xx/5xx responses
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Product>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                // Log the error (you'd use ILogger here in a real app)
                Console.WriteLine($"Error getting products: {ex.Message}");
                // You might re-throw, return null, or a custom error object/list
                return null;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Product>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Product with ID {id} not found.");
                    return null;
                }
                else
                {
                    response.EnsureSuccessStatusCode(); // Throws for other non-success status codes
                    return null; // Should not be reached if EnsureSuccessStatusCode throws
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error getting product by ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Product?> CreateProductAsync(Product product)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(product);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseApiUrl, content);

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Product>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                // You might inspect ex.StatusCode for specific HTTP error handling (e.g., 400 Bad Request)
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(product);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseApiUrl}{product.Id}", content);
                response.EnsureSuccessStatusCode();
                return true; // Indicate success
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error updating product with ID {product.Id}: {ex.Message}");
                return false; // Indicate failure
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}{id}");
                response.EnsureSuccessStatusCode();
                return true; // Indicate success
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error deleting product with ID {id}: {ex.Message}");
                return false; // Indicate failure
            }
        }
    }
}