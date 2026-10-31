
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Server.Tests;

public class UserApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public UserApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object NewUser(string email) => new
    {
        email,
        password = "password123",
        displayName = "Test User"
    };

    private static string UniqueEmail() =>
        $"test-{Guid.NewGuid():N}@example.com";

    private async Task<Uri> CreateUserAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/users", NewUser(UniqueEmail()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        return response.Headers.Location;
    }

    private static async Task AssertProblemAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            (int)expectedStatus,
            document.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task GetUsers_Returns200()
    {
        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Returns201AndLocation()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/users", NewUser(UniqueEmail()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateUser_InvalidData_Returns400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/users", new
            {
                email = "wrong-email",
                password = "123",
                displayName = ""
            });

        await AssertProblemAsync(
            response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_Returns409()
    {
        var email = UniqueEmail();

        var first = await _client.PostAsJsonAsync(
            "/api/users", NewUser(email));

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.PostAsJsonAsync(
            "/api/users", NewUser(email));

        await AssertProblemAsync(
            second, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetUser_Existing_Returns200()
    {
        var url = await CreateUserAsync();

        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_Missing_Returns404()
    {
        var response = await _client.GetAsync(
            "/api/users/2147483647");

        await AssertProblemAsync(
            response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReplaceUser_Twice_Returns204()
    {
        var url = await CreateUserAsync();

        var request = new
        {
            email = UniqueEmail(),
            password = "newpassword123",
            displayName = "Updated User"
        };

        var first = await _client.PutAsJsonAsync(url, request);
        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

        var second = await _client.PutAsJsonAsync(url, request);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);
    }

    [Fact]
    public async Task ReplaceUser_Missing_Returns404()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/users/2147483647",
            NewUser(UniqueEmail()));

        await AssertProblemAsync(
            response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReplaceUser_InvalidData_Returns400()
    {
        var url = await CreateUserAsync();

        var response = await _client.PutAsJsonAsync(
            url, new
            {
                email = "invalid",
                password = "123",
                displayName = ""
            });

        await AssertProblemAsync(
            response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReplaceUser_DuplicateEmail_Returns409()
    {
        var firstUrl = await CreateUserAsync();
        var secondEmail = UniqueEmail();

        var second = await _client.PostAsJsonAsync(
            "/api/users", NewUser(secondEmail));

        Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        var response = await _client.PutAsJsonAsync(
            firstUrl, NewUser(secondEmail));

        await AssertProblemAsync(
            response, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteUser_Twice_Returns204Then404()
    {
        var url = await CreateUserAsync();

        var first = await _client.DeleteAsync(url);
        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

        var second = await _client.DeleteAsync(url);

        await AssertProblemAsync(
            second, HttpStatusCode.NotFound);
    }
}
