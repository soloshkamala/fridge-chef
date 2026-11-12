
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class ReplaceShoppingItem
{
    public static Results<
        NoContent,
        ProblemHttpResult> Handle(
        int id,
        UpdateShoppingItemRequest request,
        ShoppingItemStore store)
    {
        if (!ShoppingItemValidation.IsValid(
            request.Name,
            request.Quantity,
            request.Unit,
            request.Category))
        {
            return TypedResults.Problem(
                statusCode: 400,
                title: "Validation failed",
                detail: "Invalid shopping item data.");
        }

        var item = store.Update(id, request);

        if (item is null)
        {
            return TypedResults.Problem(
                statusCode: 404,
                title: "Shopping item not found");
        }

        return TypedResults.NoContent();
    }
}
