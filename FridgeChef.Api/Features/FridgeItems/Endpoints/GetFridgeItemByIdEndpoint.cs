using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", Results<Ok<FridgeItemResponse>, ProblemHttpResult>
            (int id, FridgeItemsStore store) =>
        {
            var item = store.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");

            return TypedResults.Ok(item);
        })
        .ProducesProblem(404);
    }
}