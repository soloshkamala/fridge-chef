
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FridgeChef.Api.Authentication;

public sealed class E2ETestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public E2ETestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var testUser = Request.Headers["X-Test-User"].FirstOrDefault();

        var (userId, email, oid) = testUser switch
        {
            "user2" => (
                "2",
                "shopping-test2@example.com",
                "33333333-3333-3333-3333-333333333333"
            ),
            _ => (
                "1",
                "shopping-test@example.com",
                "11111111-1111-1111-1111-111111111111"
            )
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, $"E2E User {userId}"),
            new Claim(ClaimTypes.Email, email),
            new Claim("email", email),
            new Claim("oid", oid),
            new Claim("tid", "22222222-2222-2222-2222-222222222222"),
            new Claim("scp", "access_as_user"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("roles", "User"),
            new Claim("roles", "Admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}
