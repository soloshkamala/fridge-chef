using FridgeChef.Api.Features.Products.Models;
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
            var products = await _httpClient.GetFromJsonAsync<List<ProductResponse>>(
                "api/products");

            return products ?? new List<ProductResponse>();
        }
    }
}