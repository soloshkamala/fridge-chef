
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class ReplaceShoppingItem
{
    public static async Task<IResult> Handle(
        int id,
        UpdateShoppingItemRequest request,
        ShoppingItemStore store,
        CurrentUserService currentUserService,
        IHubContext<ShoppingListHub> hubContext,
        HttpContext httpContext)
    {
        if (!ShoppingItemValidation.IsValid(
            request.Name,
            request.Quantity,
            request.Unit,
            request.Category))
        {
            return Results.BadRequest("Некоректні дані товару.");
        }

        var userId = await currentUserService.GetUserIdAsync(
            httpContext.User);

        if (userId == null)
            return Results.Unauthorized();

        var item = await store.UpdateAsync(
            id,
            request,
            userId.Value);

        if (item == null)
            return Results.NotFound();

        await hubContext.Clients
            .Group(ShoppingListHubGroups.ForUser(userId.Value))
            .SendAsync("ShoppingItemUpdated", item.Id);

        return Results.NoContent();
    }
}
