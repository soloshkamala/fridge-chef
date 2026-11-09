using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class ReplaceFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async Task<Results<NoContent, ProblemHttpResult>>
            (int id, SaveFridgeItemRequest request, FridgeChefDbContext dbContext, ClaimsPrincipal user) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Unit) ||
                request.Quantity <= 0)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Invalid product data");
            }

            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var item = await dbContext.FridgeItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            var productName = request.Name.Trim();
            var unitName = request.Unit.Trim();

            var product = await dbContext.Products
                .FirstOrDefaultAsync(x => x.Name.ToLower() == productName.ToLower());

            if (product == null)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Product not found in catalog");
            }

            var unit = await dbContext.Units
                .FirstOrDefaultAsync(x => x.ShortName == unitName);

            if (unit == null)
            {
                return TypedResults.Problem(
                    statusCode: 400,
                    title: "Unit not found");
            }

            var duplicateExists = await dbContext.FridgeItems.AnyAsync(x =>
                x.Id != id &&
                x.UserId == userId &&
                x.ProductId == product.Id &&
                x.UnitId == unit.Id);

            if (duplicateExists)
            {
                return TypedResults.Problem(
                    statusCode: 409,
                    title: "Product already exists");
            }

            item.ProductId = product.Id;
            item.UnitId = unit.Id;
            item.Quantity = request.Quantity;

            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .ProducesProblem(400)
        .ProducesProblem(401)
        .ProducesProblem(404)
        .ProducesProblem(409);
    }
}