
using System.Net;
using System.Net.Http.Json;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FridgeChef.Api.Features.Profiles;
using Xunit;

namespace Server.Tests;

public class ProfileApiTests : IAsyncLifetime
{
    private const string TestEmail = "shopping-test@example.com";
    private const string TestObjectId = "11111111-1111-1111-1111-111111111111";
    private const string TestTenantId = "22222222-2222-2222-2222-222222222222";

    private readonly Testcontainers.PostgreSql.PostgreSqlContainer _postgres =
        new Testcontainers.PostgreSql.PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("fridgechef_profile_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private HttpClient _client = null!;
    private ApiFactory _factory = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new ApiFactory
        {
            ConnectionString = _postgres.GetConnectionString(),
            UseTestAuthentication = true
        };

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task EnsureUserAsync(string name)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        var user = await db.Users
            .FirstOrDefaultAsync(
                u => u.Email.ToLower() == TestEmail.ToLower());

        if (user == null)
        {
            var nameParts = name.Split(
                ' ',
                2,
                StringSplitOptions.RemoveEmptyEntries);

            user = new User
            {
                Email = TestEmail,
                FirstName = nameParts.Length > 0 ? nameParts[0] : "",
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                EntraObjectId = TestObjectId,
                EntraTenantId = TestTenantId
            };

            db.Users.Add(user);
        }
        else
        {
            user.EntraObjectId = TestObjectId;
            user.EntraTenantId = TestTenantId;
        }

        await db.SaveChangesAsync();

       
        var savedUser = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == TestEmail);

        Assert.NotNull(savedUser);
        Assert.Equal(TestObjectId, savedUser.EntraObjectId);
        Assert.Equal(TestTenantId, savedUser.EntraTenantId);
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
        await EnsureUserAsync("Test User");

        var response = await _client.GetAsync("/api/profiles/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProfile_Returns201Created()
    {
        await EnsureUserAsync("Test User");

        var request = new
        {
            name = "Test User",
            email = TestEmail,
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
        await EnsureUserAsync("Test User");

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
        await EnsureUserAsync("First User");

        var firstProfile = new
        {
            name = "First User",
            email = TestEmail,
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
            email = TestEmail,
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
        await EnsureUserAsync("Original User");

        var newProfile = new
        {
            name = "Original User",
            email = TestEmail,
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
            email = TestEmail,
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
        await EnsureUserAsync("Delete Test User");

        var newProfile = new
        {
            name = "Delete Test User",
            email = TestEmail,
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
        await EnsureUserAsync("Concurrent User");

        var request = new
        {
            name = "Concurrent User",
            email = TestEmail,
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

    [Fact]
    public async Task GetMyProfile_WhenProfileDoesNotExist_ReturnsOkWithNull()
    {
        await EnsureUserAsync("Test User");

        var response = await _client.GetAsync("/api/profiles/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();

        Assert.Null(profile);
    }

    [Fact]
    public async Task GetMyProfile_WhenProfileExists_ReturnsFullProfile()
    {
        await EnsureUserAsync("Test User");

        var request = new
        {
            name = "Test User",
            email = TestEmail,
            diets = new[] { "Vegetarian" },
            favoriteIngredients = "Tomatoes"
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/profiles", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await _client.GetAsync("/api/profiles/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();

        Assert.NotNull(profile);
        Assert.True(profile.Id > 0);
        Assert.Equal("Test User", profile.Name);
        Assert.Equal(TestEmail, profile.Email);
        Assert.Contains("Vegetarian", profile.Diets);
        Assert.Equal("Tomatoes", profile.FavoriteIngredients);
    }
}
