
using System.Net;
using System.Net.Http.Json;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Server.Tests;

public class ProfileApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public ProfileApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Створюємо користувача в PostgreSQL перед створенням профілю
    private async Task EnsureUserAsync(string email, string name)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        var existingUser = await db.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (existingUser != null)
            return;

        var nameParts = name.Split(
            ' ',
            2,
            StringSplitOptions.RemoveEmptyEntries);

        var user = new User
        {
            Email = email,
            FirstName = nameParts.Length > 0 ? nameParts[0] : "",
            LastName = nameParts.Length > 1 ? nameParts[1] : ""
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetProfiles_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/profiles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/profiles/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProfile_Returns201Created()
    {
        var email = $"newuser-{Guid.NewGuid():N}@example.com";

        await EnsureUserAsync(email, "Test User");

        var request = new
        {
            name = "Test User",
            email,
            diets = new[] { "Vegetarian" },
            favoriteIngredients = "Tomatoes"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/profiles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateProfile_WithInvalidData_Returns400()
    {
        var request = new
        {
            name = "",
            email = "wrong-email",
            diets = Array.Empty<string>(),
            favoriteIngredients = ""
        };

        var response = await _client.PostAsJsonAsync(
            "/api/profiles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProfile_WithDuplicateEmail_Returns409()
    {
        var email = $"duplicate-{Guid.NewGuid():N}@example.com";

        await EnsureUserAsync(email, "First User");

        var firstProfile = new
        {
            name = "First User",
            email,
            diets = Array.Empty<string>(),
            favoriteIngredients = "Tomatoes"
        };

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/profiles", firstProfile);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var secondProfile = new
        {
            name = "Second User",
            email,
            diets = Array.Empty<string>(),
            favoriteIngredients = "Apples"
        };

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/profiles", secondProfile);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_Returns200Ok()
    {
        var email = $"update-{Guid.NewGuid():N}@example.com";

        await EnsureUserAsync(email, "Original User");

        var newProfile = new
        {
            name = "Original User",
            email,
            diets = Array.Empty<string>(),
            favoriteIngredients = "Tomatoes"
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/profiles", newProfile);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var profileUrl = createResponse.Headers.Location;

        Assert.NotNull(profileUrl);

        var updatedProfile = new
        {
            name = "Updated User",
            email,
            diets = new[] { "Vegetarian" },
            favoriteIngredients = "Cheese"
        };

        var updateResponse = await _client.PutAsJsonAsync(
            profileUrl, updatedProfile);

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProfile_Returns204AndThen404()
    {
        var email = $"delete-{Guid.NewGuid():N}@example.com";

        await EnsureUserAsync(email, "Delete Test User");

        var newProfile = new
        {
            name = "Delete Test User",
            email,
            diets = Array.Empty<string>(),
            favoriteIngredients = "Apples"
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/profiles", newProfile);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var profileUrl = createResponse.Headers.Location;

        Assert.NotNull(profileUrl);

        var deleteResponse = await _client.DeleteAsync(profileUrl);

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync(profileUrl);

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProfile_ConcurrentDuplicateEmail_Returns201And409()
    {
        var email = $"concurrent-{Guid.NewGuid():N}@example.com";

        await EnsureUserAsync(email, "Concurrent User");

        var request = new
        {
            name = "Concurrent User",
            email,
            diets = Array.Empty<string>(),
            favoriteIngredients = "Tomatoes"
        };

        var tasks = new[]
        {
            _client.PostAsJsonAsync("/api/profiles", request),
            _client.PostAsJsonAsync("/api/profiles", request)
        };

        var responses = await Task.WhenAll(tasks);

        var statusCodes = responses
            .Select(response => response.StatusCode)
            .OrderBy(status => status)
            .ToArray();

        Assert.Equal(
            new[]
            {
                HttpStatusCode.Created,
                HttpStatusCode.Conflict
            },
            statusCodes);
    }
}
