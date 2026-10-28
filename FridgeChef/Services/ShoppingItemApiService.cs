
using System.Net.Http.Json;
using FridgeChef.Models;

namespace FridgeChef.Services;

public class ShoppingItemApiService
{
    private readonly HttpClient _http;

    public ShoppingItemApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ShoppingItem>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<ShoppingItem>>(
            "api/shopping-items") ?? new List<ShoppingItem>();
    }

    public async Task<ShoppingItem> AddAsync(ShoppingItem item)
    {
        var request = new
        {
            item.Name,
            item.Quantity,
            item.Unit,
            item.Category
        };

        var response = await _http.PostAsJsonAsync(
            "api/shopping-items", request);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ShoppingItem>())!;
    }

    public async Task UpdateAsync(ShoppingItem item)
    {
        var request = new
        {
            item.Name,
            item.Quantity,
            item.Unit,
            item.Category,
            item.IsPurchased,
            item.IsInFridge
        };

        var response = await _http.PutAsJsonAsync(
            $"api/shopping-items/{item.Id}", request);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync(
            $"api/shopping-items/{id}");

        response.EnsureSuccessStatusCode();
    }
}

