
namespace FridgeChef.Api.Features.Users;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(
        this RouteGroupBuilder group)
    {
        group.MapGet("/", GetUsers.Handle)
            .WithName("GetUsers")
            .Produces<List<UserResponse>>(200);

        group.MapGet("/{id:int}", GetUser.Handle)
            .WithName("GetUser")
            .Produces<UserResponse>(200)
            .ProducesProblem(404);

        group.MapPost("/", CreateUser.Handle)
            .WithName("CreateUser")
            .Produces<UserResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(409);

        group.MapPut("/{id:int}", ReplaceUser.Handle)
            .WithName("ReplaceUser")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(409);

        group.MapDelete("/{id:int}", DeleteUser.Handle)
            .WithName("DeleteUser")
            .Produces(204)
            .ProducesProblem(404);

        return group;
    }
}
