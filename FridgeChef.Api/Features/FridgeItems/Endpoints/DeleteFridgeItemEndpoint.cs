using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class DeleteFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", Results<NoContent, ProblemHttpResult>
            (int id, FridgeItemsStore store) =>
        {
            var item = store.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");

            store.Items.Remove(item);

            return TypedResults.NoContent();
        })
        .ProducesProblem(404);
    }
}