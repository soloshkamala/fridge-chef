
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using FridgeChef.Models;

namespace FridgeChef.Services;

public class ShoppingItemApiService
{
    private readonly HttpClient _http;

    public ShoppingItemApiService(HttpClient http)
    {
        _http = http;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string url,
        HttpContent? content = null)
    {
        using var request = new HttpRequestMessage(method, url);

        request.Content = content;

        request.SetBrowserRequestCredentials(
            BrowserRequestCredentials.Include);

        return await _http.SendAsync(request);
    }

    public async Task<List<ShoppingItem>> GetAllAsync()
    {
        using var response = await SendRequestAsync(
            HttpMethod.Get,
            "api/shopping-items");

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<List<ShoppingItem>>()
            ?? new List<ShoppingItem>();
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

        using var content = JsonContent.Create(request);

        using var response = await SendRequestAsync(
            HttpMethod.Post,
            "api/shopping-items",
            content);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ShoppingItem>())!;
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

        using var content = JsonContent.Create(request);

        using var response = await SendRequestAsync(
            HttpMethod.Put,
            $"api/shopping-items/{item.Id}",
            content);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        using var response = await SendRequestAsync(
            HttpMethod.Delete,
            $"api/shopping-items/{id}");

        response.EnsureSuccessStatusCode();
    }
}
