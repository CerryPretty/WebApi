using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebUI.Models; // Убедитесь, что это пространство имен соответствует модели Order вашего WebUI

namespace WebUI.Services
{
    public class OrderApiClient
    {
        private readonly HttpClient _httpClient;
        // ВАЖНО: Замените на фактический URL, по которому работает ваш WebApi!
        private readonly string _baseApiUrl = "http://localhost:5000/api/orders/";

        public OrderApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        public async Task<List<Order>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync(_baseApiUrl);
            response.EnsureSuccessStatusCode(); // Выбрасывает исключение, если статус 4xx или 5xx
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Order>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Order>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var jsonContent = JsonSerializer.Serialize(order);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseApiUrl, content);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Order>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var jsonContent = JsonSerializer.Serialize(order);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseApiUrl}{order.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}