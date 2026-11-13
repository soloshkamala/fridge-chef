
namespace FridgeChef.Api.Features.ShoppingItems;

public static class ShoppingItemEndpoints
{
    public static RouteGroupBuilder MapShoppingItemEndpoints(
        this RouteGroupBuilder group)
    {
        // All shopping item endpoints require authentication
        group.RequireAuthorization();

        group.MapGet("/", GetShoppingItems.Handle)
            .WithName("GetShoppingItems")
            .Produces<List<ShoppingItemResponse>>(200);

        group.MapGet("/{id:int}", GetShoppingItem.Handle)
            .WithName("GetShoppingItem")
            .Produces<ShoppingItemResponse>(200)
            .ProducesProblem(404);

        group.MapPost("/", CreateShoppingItem.Handle)
            .WithName("CreateShoppingItem")
            .Produces<ShoppingItemResponse>(201)
            .ProducesProblem(400);

        group.MapPut("/{id:int}", ReplaceShoppingItem.Handle)
            .WithName("ReplaceShoppingItem")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapDelete("/{id:int}", DeleteShoppingItem.Handle)
            .WithName("DeleteShoppingItem")
            .Produces(204)
            .ProducesProblem(404);

        return group;
    }
}
