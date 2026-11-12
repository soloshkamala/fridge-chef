
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class DeleteShoppingItem
{
    public static Results<
        NoContent,
        ProblemHttpResult> Handle(
        int id,
        ShoppingItemStore store)
    {
        if (!store.Delete(id))
        {
            return TypedResults.Problem(
                statusCode: 404,
                title: "Shopping item not found");
        }

        return TypedResults.NoContent();
    }
}
