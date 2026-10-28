using FridgeChef.Api.Features.FridgeItems.Endpoints;

namespace FridgeChef.Api.Features.FridgeItems;

public static class FridgeItemsEndpoints
{
    public static RouteGroupBuilder MapFridgeItemsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/fridge-items")
            .WithTags("FridgeItems");

        GetFridgeItemsEndpoint.Map(group);
        GetFridgeItemByIdEndpoint.Map(group);
        CreateFridgeItemEndpoint.Map(group);
        ReplaceFridgeItemEndpoint.Map(group);
        DeleteFridgeItemEndpoint.Map(group);

        return group;
    }
}