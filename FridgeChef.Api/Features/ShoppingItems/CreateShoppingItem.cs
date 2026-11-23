
using FridgeChef.Api.Services;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class CreateShoppingItem
{
    public static async Task<IResult> Handle(
        CreateShoppingItemRequest request,
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

        var item = await store.AddAsync(request, userId.Value);

        if (item == null)
        {
            return Results.BadRequest(
                "Товар або одиниця вимірювання відсутні в каталозі або несумісні.");
        }

        await hubContext.Clients
            .Group(ShoppingListHubGroups.ForUser(userId.Value))
            .SendAsync("ShoppingItemAdded", item.Id);

        return Results.Created(
            $"/api/shopping-items/{item.Id}",
            item);
    }
}
