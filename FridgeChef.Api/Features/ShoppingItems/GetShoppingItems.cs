
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class GetShoppingItems
{
    public static async Task<IResult> Handle(
        ShoppingItemStore store,
        CurrentUserService currentUserService,
        HttpContext httpContext)
    {
        var userId = await currentUserService.GetUserIdAsync(
            httpContext.User);

        if (userId == null)
            return Results.Unauthorized();

        var items = await store.GetAllAsync(userId.Value);

        return Results.Ok(items);
    }
}
