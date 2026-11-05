
using System.Security.Claims;
using FridgeChef.Api.Features.Users;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.Auth;

public static class GetSession
{
    public static Results<
        Ok<AuthResponse>,
        ProblemHttpResult> Handle(
        ClaimsPrincipal principal,
        UserStore store)
    {
        var idText = principal.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(idText, out var id))
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Unauthorized");
        }

        var user = store.GetById(id);

        if (user is null)
        {
            return TypedResults.Problem(
                statusCode: 401,
                title: "Session is no longer valid");
        }

        return TypedResults.Ok(
            new AuthResponse(
                user.Id,
                user.Email,
                user.DisplayName));
    }
}
