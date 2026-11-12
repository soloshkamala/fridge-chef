
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class CreateShoppingItem
{
    public static Results<
        Created<ShoppingItemResponse>,
        ProblemHttpResult> Handle(
        CreateShoppingItemRequest request,
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

        var item = store.Add(request);

        return TypedResults.Created(
            $"/api/shopping-items/{item.Id}",
            item);
    }
}
