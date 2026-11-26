using System.Net.Http.Json;
using FridgeChef.Models;

namespace FridgeChef.Services
{
    public class FridgeItemsApiService
    {
        private readonly HttpClient httpClient;
        private const string ApiUrl = "api/fridge-items";

        public FridgeItemsApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<FridgeItem>> GetItemsAsync()
        {
            using var request = LocalizedApiRequest.Create(
                HttpMethod.Get, ApiUrl);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var items = await response.Content
                .ReadFromJsonAsync<List<ApiFridgeItem>>();

            if (items == null)
            {
                return new List<FridgeItem>();
            }

            return items.Select(item => new FridgeItem
            {
                Id = item.Id,
                Ingredient = new Ingredient
                {
                    Id = 0,
                    Name = item.Name,
                    DisplayName = item.DisplayName
                },
                Quantity = (double)item.Quantity,
                Unit = item.Unit,
                DisplayUnit = item.DisplayUnit
            }).ToList();
        }

        public async Task<FridgeItem> AddItemAsync(FridgeItem item)
        {
            var data = new
            {
                item.Ingredient.Name,
                Quantity = (decimal)item.Quantity,
                item.Unit
            };

            using var request = LocalizedApiRequest.Create(
                HttpMethod.Post, ApiUrl);

            request.Content = JsonContent.Create(data);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var savedItem = await response.Content
                .ReadFromJsonAsync<ApiFridgeItem>();

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
                    Name = savedItem.Name,
                    DisplayName = savedItem.DisplayName
                },
                Quantity = (double)savedItem.Quantity,
                Unit = savedItem.Unit,
                DisplayUnit = savedItem.DisplayUnit
            };
        }

        public async Task UpdateItemAsync(FridgeItem item)
        {
            var request = new
            {
                Name = item.Ingredient.Name,
                Quantity = (decimal)item.Quantity,
                Unit = item.Unit
            };

            var response = await httpClient.PutAsJsonAsync(
                $"{ApiUrl}/{item.Id}",
                request
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteItemAsync(int id)
        {
            var response = await httpClient.DeleteAsync(
                $"{ApiUrl}/{id}"
            );

            response.EnsureSuccessStatusCode();
        }

        private class ApiFridgeItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string DisplayName { get; set; } = "";
            public decimal Quantity { get; set; }
            public string Unit { get; set; } = "";
            public string DisplayUnit { get; set; } = "";
        }
    }
}
