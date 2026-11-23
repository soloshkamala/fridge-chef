using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Server.Tests;

public sealed class AuthApiTests
{
    private const string Audience = "api://fridgechef-api-test";
    private const string Issuer =
        "https://login.microsoftonline.com/test-tenant/v2.0";

    private static readonly RSA Rsa = RSA.Create(2048);

    private static readonly RsaSecurityKey SigningKey =
        new(Rsa.ExportParameters(false));

    private static readonly SigningCredentials Credentials =
        new(new RsaSecurityKey(Rsa), SecurityAlgorithms.RsaSha256);

    private sealed class JwtFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ClientOrigin"] = "http://localhost:5050",
                        ["AzureAd:Instance"] =
                            "https://login.microsoftonline.com/",
                        ["AzureAd:TenantId"] = "test-tenant",
                        ["AzureAd:ClientId"] = "fridgechef-api-test",
                        ["AzureAd:Audience"] = Audience
                    });
            });

            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.MapInboundClaims = false;
                        options.Authority = null;
                        options.MetadataAddress = string.Empty;
                        options.ConfigurationManager = null;

                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = Issuer,
                                ValidateAudience = true,
                                ValidAudience = Audience,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = SigningKey,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromSeconds(5),
                                RoleClaimType = "roles"
                            };
                    });
            });
        }
    }

    private static string CreateToken(
        string audience = Audience,
        string role = "User",
        bool expired = false)
    {
        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim("oid", Guid.NewGuid().ToString()),
            new Claim("tid", "test-tenant"),
            new Claim("scp", "access_as_user"),
            new Claim("roles", role)
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: audience,
            claims: claims,
            notBefore: expired ? now.AddHours(-2) : now.AddMinutes(-1),
            expires: expired ? now.AddHours(-1) : now.AddHours(1),
            signingCredentials: Credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static HttpClient CreateClient(
        JwtFactory factory,
        string token)
    {
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    [Fact]
    public async Task RequestWithoutToken_Returns401()
    {
        using var factory = new JwtFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WrongAudience_Returns401()
    {
        using var factory = new JwtFactory();
        using var client = CreateClient(
            factory,
            CreateToken(audience: "api://another-api"));

        using var response = await client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        using var factory = new JwtFactory();
        using var client = CreateClient(
            factory,
            CreateToken(expired: true));

        using var response = await client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserRole_CannotCreateCourse_Returns403()
    {
        using var factory = new JwtFactory();
        using var client = CreateClient(
            factory,
            CreateToken(role: "User"));

        using var response = await client.PostAsJsonAsync(
            "/api/courses",
            new
            {
                Code = "AUTH101",
                Title = "Authorization Testing",
                Credits = 3,
                TeacherId = 1
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminRole_CanCreateCourse_Returns201()
    {
        using var factory = new JwtFactory();
        using var client = CreateClient(
            factory,
            CreateToken(role: "Admin"));

        using var response = await client.PostAsJsonAsync(
            "/api/courses",
            new
            {
                Code = "AUTH" + Guid.NewGuid().ToString("N")[..6],
                Title = "Authorization Testing",
                Credits = 3,
                TeacherId = 1
            });

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("ADMIN RESPONSE STATUS: " + response.StatusCode);
        Console.WriteLine("ADMIN RESPONSE BODY: " + responseBody);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
