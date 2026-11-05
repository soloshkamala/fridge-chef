
namespace FridgeChef.Api.Features.Auth;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(
        this RouteGroupBuilder group)
    {
        group.MapPost("/login", (Delegate)Login.Handle)
            .WithName("Login")
            .Produces<AuthResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401);

        group.MapGet("/session", (Delegate)GetSession.Handle)
            .WithName("GetSession")
            .RequireAuthorization()
            .Produces<AuthResponse>(200)
            .ProducesProblem(401);

        group.MapPost("/logout", (Delegate)Logout.Handle)
            .WithName("Logout")
            .RequireAuthorization()
            .Produces(204)
            .ProducesProblem(401);

        return group;
    }
}
