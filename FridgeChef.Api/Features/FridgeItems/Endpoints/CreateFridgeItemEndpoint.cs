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

            var existingItem = store.Items.FirstOrDefault(x =>
                x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));

            if (existingItem != null)
            {
                if (existingItem.Unit.Equals(request.Unit, StringComparison.OrdinalIgnoreCase))
                {
                    existingItem.Quantity += request.Quantity;
                }
                else if (existingItem.Unit == "л" && request.Unit == "мл")
                {
                    existingItem.Quantity += request.Quantity / 1000;
                }
                else if (existingItem.Unit == "мл" && request.Unit == "л")
                {
                    existingItem.Quantity += request.Quantity * 1000;
                }
                else if (existingItem.Unit == "кг" && request.Unit == "г")
                {
                    existingItem.Quantity += request.Quantity / 1000;
                }
                else if (existingItem.Unit == "г" && request.Unit == "кг")
                {
                    existingItem.Quantity += request.Quantity * 1000;
                }
                else
                {
                    return TypedResults.Problem(
                        statusCode: 400,
                        title: "Incompatible units");
                }

                return TypedResults.Created(
                    $"/api/fridge-items/{existingItem.Id}", existingItem);
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