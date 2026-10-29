using System.Net.Http.Json;
using FridgeChef.Models;

namespace FridgeChef.Services
{
    public class FridgeItemsApiService
    {
        private readonly HttpClient httpClient;
        private const string ApiUrl = "http://localhost:5039/api/fridge-items";

        public FridgeItemsApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<FridgeItem>> GetItemsAsync()
        {
            var response = await httpClient.GetFromJsonAsync<List<ApiFridgeItem>>(ApiUrl);

            if (response == null)
            {
                return new List<FridgeItem>();
            }

            return response.Select(item => new FridgeItem
            {
                Id = item.Id,
                Ingredient = new Ingredient
                {
                    Id = 0,
                    Name = item.Name
                },
                Quantity = (double)item.Quantity,
                Unit = item.Unit
            }).ToList();
        }

        public async Task<FridgeItem> AddItemAsync(FridgeItem item)
        {
            var request = new
            {
                item.Ingredient.Name,
                Quantity = (decimal)item.Quantity,
                item.Unit
            };

            var response = await httpClient.PostAsJsonAsync(ApiUrl, request);

            response.EnsureSuccessStatusCode();

            var savedItem = await response.Content.ReadFromJsonAsync<ApiFridgeItem>();

            if (savedItem == null)
            {
                throw new Exception("Не вдалося отримати збережений продукт.");
            }

            return new FridgeItem
            {
                Id = savedItem.Id,
                Ingredient = new Ingredient
                {
                    Id = 0,
                    Name = savedItem.Name
                },
                Quantity = (double)savedItem.Quantity,
                Unit = savedItem.Unit
            };
        }

        private class ApiFridgeItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public decimal Quantity { get; set; }
            public string Unit { get; set; } = "";
        }
    }
}