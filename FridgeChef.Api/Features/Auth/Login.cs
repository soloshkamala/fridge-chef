
using System.Security.Claims;
using FridgeChef.Api.Features.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Auth;

public static class Login
{
    public static async Task<Results<
        Ok<AuthResponse>,
        ProblemHttpResult>> Handle(
        LoginRequest request,
        HttpContext context,
        UserStore store,
        UserPasswordService passwords)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return TypedResults.Problem(
                statusCode: 400,
                title: "Validation failed",
                detail: "Email and password are required.");
        }

        var user = store.GetByEmail(request.Email.Trim());

        if (user is null ||
            !passwords.Verify(user, request.Password))
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Invalid email or password");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return TypedResults.Ok(
            new AuthResponse(
                user.Id,
                user.Email,
                user.DisplayName));
    }
}
