using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class ReplaceFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", Results<NoContent, ProblemHttpResult>
            (int id, SaveFridgeItemRequest request, FridgeItemsStore store) =>
        {
            var item = store.Items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                request.Quantity <= 0)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Invalid product data");
            }

            if (store.Items.Any(x =>
                x.Id != id &&
                x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase) &&
                x.Unit.Equals(request.Unit, StringComparison.OrdinalIgnoreCase)))
            {
                return TypedResults.Problem(
                    statusCode: 409,
                    title: "Product already exists");
            }

            item.Name = request.Name;
            item.Quantity = request.Quantity;
            item.Unit = request.Unit;

            return TypedResults.NoContent();
        })
        .ProducesProblem(400)
        .ProducesProblem(404)
        .ProducesProblem(409);
    }
}