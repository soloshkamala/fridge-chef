
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Server.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public string? ConnectionString { get; set; }

    public bool UseTestAuthentication { get; set; }

    // ������ �������� ����������� ��� xUnit
    public ApiFactory()
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            ConnectionString
                    });
            });
        }

        if (UseTestAuthentication)
        {
            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<AuthorizationOptions>(options =>
                {
                    foreach (var name in new[] { "ApiRead", "ApiWrite", "ApiAdmin" })
                    {
                        var policy = options.GetPolicy(name);

                        if (policy is null)
                            continue;

                        var updated = new AuthorizationPolicyBuilder("Test")
                            .Combine(policy)
                            .Build();

                        options.AddPolicy(name, updated);
                    }
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test",
                    options => { });
            });
        }
    }
}

public sealed class TestAuthHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(
                ClaimTypes.Email,
                "shopping-test@example.com"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim("scp", "access_as_user"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("roles", "User"),
            new Claim("roles", "Admin"),
            new Claim("oid", "11111111-1111-1111-1111-111111111111"),
            new Claim("tid", "22222222-2222-2222-2222-222222222222")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}
