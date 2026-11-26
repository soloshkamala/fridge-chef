using FridgeChef.Models;
using System.Net.Http.Json;

namespace FridgeChef.Services
{
    public class ProductsApiService
    {
        private readonly HttpClient _httpClient;

        public ProductsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            using var request = LocalizedApiRequest.Create(
                HttpMethod.Get, "api/products");

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var products = await response.Content
                .ReadFromJsonAsync<List<ProductResponse>>();

            return products ?? new List<ProductResponse>();
        }
    }
}
