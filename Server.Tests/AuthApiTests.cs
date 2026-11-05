
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Server.Tests;

public class AuthApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AuthApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() =>
        _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

    private static string UniqueEmail() =>
        $"auth-{Guid.NewGuid():N}@example.com";

    private static async Task RegisterAsync(
        HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/users",
            new
            {
                email,
                password = "password123",
                displayName = "Auth Test"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task LoginAsync(
        HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password = "password123"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
    public async Task Login_ValidCredentials_Returns200()
    {
        using var client = CreateClient();
        var email = UniqueEmail();

        await RegisterAsync(client, email);

        await LoginAsync(client, email);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        using var client = CreateClient();
        var email = UniqueEmail();

        await RegisterAsync(client, email);

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password = "wrongpassword"
            });

        await AssertProblemAsync(
            response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_MissingCredentials_Returns400()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email = "",
                password = ""
            });

        await AssertProblemAsync(
            response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Session_WithoutLogin_Returns401()
    {
        using var client = CreateClient();

        var response = await client.GetAsync(
            "/api/auth/session");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Session_AfterLogin_Returns200()
    {
        using var client = CreateClient();
        var email = UniqueEmail();

        await RegisterAsync(client, email);
        await LoginAsync(client, email);

        var response = await client.GetAsync(
            "/api/auth/session");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Logout_AfterLogin_Returns204()
    {
        using var client = CreateClient();
        var email = UniqueEmail();

        await RegisterAsync(client, email);
        await LoginAsync(client, email);

        var response = await client.PostAsync(
            "/api/auth/logout", null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutLogin_Returns401()
    {
        using var client = CreateClient();

        var response = await client.PostAsync(
            "/api/auth/logout", null);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Session_AfterLogout_Returns401()
    {
        using var client = CreateClient();
        var email = UniqueEmail();

        await RegisterAsync(client, email);
        await LoginAsync(client, email);

        var logout = await client.PostAsync(
            "/api/auth/logout", null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            logout.StatusCode);

        var session = await client.GetAsync(
            "/api/auth/session");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            session.StatusCode);
    }

[Fact]
public async Task Session_WithoutLogin_ReturnsProblemDetails()
{
    using var client = CreateClient();

    var response = await client.GetAsync(
        "/api/auth/session");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);

    Assert.Equal(
        "application/problem+json",
        response.Content.Headers.ContentType?.MediaType);

    var body = await response.Content.ReadAsStringAsync();

    using var document = JsonDocument.Parse(body);

    Assert.Equal(
        401,
        document.RootElement.GetProperty("status").GetInt32());
}

}
