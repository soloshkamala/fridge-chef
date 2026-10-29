using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", (FridgeItemsStore store) =>
        {
            return TypedResults.Ok(store.Items);
        });
    }
}