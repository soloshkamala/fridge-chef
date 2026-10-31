
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FridgeChef.Services;

public class AuthService
{
    private readonly HttpClient http;

    public AuthService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<bool> LoginAsync(
        string email,
        string password)
    {
        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/auth/login"
            );

            request.Content = JsonContent.Create(new
            {
                Email = email,
                Password = password
            });

            request.SetBrowserRequestCredentials(
                BrowserRequestCredentials.Include
            );

            using var response = await http.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
