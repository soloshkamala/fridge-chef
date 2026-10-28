
using Microsoft.AspNetCore.Mvc;

namespace FridgeChef.Api.Features.ShoppingItems;

public static class ShoppingItemEndpoints
{
    public static RouteGroupBuilder MapShoppingItemEndpoints(
        this RouteGroupBuilder group)
    {
        group.MapGet("/", (ShoppingItemStore store) =>
            Results.Ok(store.GetAll()));

        group.MapGet("/{id:int}", (int id, ShoppingItemStore store) =>
        {
            var item = store.GetById(id);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        });

        group.MapPost("/", (
            [FromBody] CreateShoppingItemRequest request,
            ShoppingItemStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                string.IsNullOrWhiteSpace(request.Category) ||
                request.Name.Trim().Length > 100 ||
                request.Unit.Trim().Length > 30 ||
                request.Category.Trim().Length > 50 ||
                request.Quantity < 0.01 ||
                request.Quantity > 1000 ||
                double.IsNaN(request.Quantity) ||
                double.IsInfinity(request.Quantity))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["request"] = ["Перевірте назву, кількість, одиницю та категорію."]
                    });
            }

            var item = store.Add(request);

            return Results.Created(
                $"/api/shopping-items/{item.Id}", item);
        });

        group.MapPut("/{id:int}", (
            int id,
            [FromBody] UpdateShoppingItemRequest request,
            ShoppingItemStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                string.IsNullOrWhiteSpace(request.Category) ||
                request.Name.Trim().Length > 100 ||
                request.Unit.Trim().Length > 30 ||
                request.Category.Trim().Length > 50 ||
                request.Quantity < 0.01 ||
                request.Quantity > 1000 ||
                double.IsNaN(request.Quantity) ||
                double.IsInfinity(request.Quantity))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["request"] = ["Перевірте дані продукту."]
                    });
            }

            var item = store.Update(id, request);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        });

        group.MapDelete("/{id:int}", (int id, ShoppingItemStore store) =>
            store.Delete(id)
                ? Results.NoContent()
                : Results.NotFound());

        return group;
    }
}
