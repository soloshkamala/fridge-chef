using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class CreateFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async Task<Results<Created<FridgeItemResponse>, ProblemHttpResult>>
            (SaveFridgeItemRequest request, FridgeChefDbContext dbContext, ClaimsPrincipal user) =>
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

            var existingItem = await dbContext.FridgeItems
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == product.Id);

            if (existingItem != null)
            {
                if (existingItem.Unit.ShortName == unitName)
                {
                    existingItem.Quantity += request.Quantity;
                }
                else if (existingItem.Unit.ShortName == "л" && unitName == "мл")
                {
                    existingItem.Quantity += request.Quantity / 1000;
                }
                else if (existingItem.Unit.ShortName == "мл" && unitName == "л")
                {
                    existingItem.Quantity += request.Quantity * 1000;
                }
                else if (existingItem.Unit.ShortName == "кг" && unitName == "г")
                {
                    existingItem.Quantity += request.Quantity / 1000;
                }
                else if (existingItem.Unit.ShortName == "г" && unitName == "кг")
                {
                    existingItem.Quantity += request.Quantity * 1000;
                }
                else
                {
                    return TypedResults.Problem(
                        statusCode: 400,
                        title: "Incompatible units");
                }

                await dbContext.SaveChangesAsync();

                var response = new FridgeItemResponse
                {
                    Id = existingItem.Id,
                    Name = existingItem.Product.Name,
                    Quantity = existingItem.Quantity,
                    Unit = existingItem.Unit.ShortName
                };

                return TypedResults.Created(
                    $"/api/fridge-items/{existingItem.Id}", response);
            }

            var item = new FridgeItem
            {
                UserId = userId,
                ProductId = product.Id,
                UnitId = unit.Id,
                Quantity = request.Quantity
            };

            dbContext.FridgeItems.Add(item);
            await dbContext.SaveChangesAsync();

            var newResponse = new FridgeItemResponse
            {
                Id = item.Id,
                Name = product.Name,
                Quantity = item.Quantity,
                Unit = unit.ShortName
            };

            return TypedResults.Created(
                $"/api/fridge-items/{item.Id}", newResponse);
        })
        .ProducesProblem(400)
        .ProducesProblem(401);
    }
}