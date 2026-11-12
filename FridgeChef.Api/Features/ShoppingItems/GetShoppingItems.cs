
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class GetShoppingItems
{
    public static Ok<List<ShoppingItemResponse>> Handle(
        ShoppingItemStore store)
    {
        return TypedResults.Ok(store.GetAll());
    }
}
