
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class DeleteShoppingItem
{
    public static async Task<IResult> Handle(
        int id,
        ShoppingItemStore store,
        CurrentUserService currentUserService,
        IHubContext<ShoppingListHub> hubContext,
        HttpContext httpContext)
    {
        var userId = await currentUserService.GetUserIdAsync(
            httpContext.User);

        if (userId == null)
            return Results.Unauthorized();

        var deleted = await store.DeleteAsync(
            id,
            userId.Value);

        if (!deleted)
            return Results.NotFound();

        await hubContext.Clients
            .Group(ShoppingListHubGroups.ForUser(userId.Value))
            .SendAsync("ShoppingItemDeleted", id);

        return Results.NoContent();
    }
}
