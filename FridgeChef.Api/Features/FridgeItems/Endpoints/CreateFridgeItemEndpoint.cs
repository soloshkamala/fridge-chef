using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class CreateFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Results<Created<FridgeItemResponse>, ProblemHttpResult>
            (SaveFridgeItemRequest request, FridgeItemsStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                request.Quantity <= 0)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Invalid product data");
            }

            if (store.Items.Any(x =>
                x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase) &&
                x.Unit.Equals(request.Unit, StringComparison.OrdinalIgnoreCase)))
            {
                return TypedResults.Problem(
                    statusCode: 409,
                    title: "Product already exists");
            }

            var item = new FridgeItemResponse
            {
                Id = store.NextId++,
                Name = request.Name,
                Quantity = request.Quantity,
                Unit = request.Unit
            };

            store.Items.Add(item);

            return TypedResults.Created(
            $"/api/fridge-items/{item.Id}", item);
        })
        .ProducesProblem(400)
        .ProducesProblem(409);
    }
}