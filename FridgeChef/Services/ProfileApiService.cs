
using System.Net.Http.Json;

namespace FridgeChef.Services;

public record ProfileApiDto(
    int Id,
    string Name,
    string Email,
    IReadOnlyCollection<string> Diets,
    string FavoriteIngredients
);

public record ProfileApiRequest(
    string Name,
    string Email,
    IReadOnlyCollection<string> Diets,
    string FavoriteIngredients
);

public class ProfileApiService
{
    private readonly HttpClient _http;

    public ProfileApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProfileApiDto?> GetMyProfileAsync()
    {
        return await _http.GetFromJsonAsync<ProfileApiDto>(
            "api/profiles/me");
    }

    public async Task<ProfileApiDto> CreateProfileAsync(
        ProfileApiRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/profiles",
            request);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ProfileApiDto>())!;
    }

    public async Task<ProfileApiDto> UpdateProfileAsync(
        int id,
        ProfileApiRequest request)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/profiles/{id}",
            request);

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<ProfileApiDto>())!;
    }
}
