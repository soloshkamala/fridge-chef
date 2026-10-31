
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Auth;

public static class Logout
{
    public static async Task<NoContent> Handle(
        HttpContext context)
    {
        await context.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return TypedResults.NoContent();
    }
}
