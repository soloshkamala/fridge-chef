
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class GetShoppingItem
{
    public static async Task<IResult> Handle(
        int id,
        ShoppingItemStore store,
        CurrentUserService currentUserService,
        HttpContext httpContext)
    {
        var userId = await currentUserService.GetUserIdAsync(
            httpContext.User);

        if (userId == null)
            return Results.Unauthorized();

        var item = await store.GetByIdAsync(id, userId.Value);

        if (item == null)
            return Results.NotFound();

        return Results.Ok(item);
    }
}
