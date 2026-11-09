
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async Task<Results<Ok<FridgeItemResponse>, ProblemHttpResult>>
            (int id, FridgeChefDbContext dbContext, ClaimsPrincipal user) =>
        {
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var fridgeItem = await dbContext.FridgeItems
                .AsNoTracking()
                .Where(x => x.Id == id && x.UserId == userId)
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync();

            if (fridgeItem == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            var item = new FridgeItemResponse
            {
                Id = fridgeItem.Id,
                Name = fridgeItem.Product.Name,
                Quantity = fridgeItem.Quantity,
                Unit = fridgeItem.Unit.ShortName
            };

            return TypedResults.Ok(item);
        })
        .ProducesProblem(401)
        .ProducesProblem(404);
    }
}
