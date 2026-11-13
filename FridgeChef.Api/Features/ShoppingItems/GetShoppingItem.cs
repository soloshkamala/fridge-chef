
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class GetShoppingItem
{
    public static Results<
        Ok<ShoppingItemResponse>,
        ProblemHttpResult> Handle(
        int id,
        ShoppingItemStore store)
    {
        var item = store.GetById(id);

        if (item is null)
        {
            return TypedResults.Problem(
                statusCode: 404,
                title: "Shopping item not found");
        }

        return TypedResults.Ok(item);
    }
}
