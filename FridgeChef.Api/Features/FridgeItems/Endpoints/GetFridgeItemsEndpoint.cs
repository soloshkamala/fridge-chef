
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.FridgeItems.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class GetFridgeItemsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async Task<Results<Ok<List<FridgeItemResponse>>, ProblemHttpResult>>
            (FridgeChefDbContext dbContext, ClaimsPrincipal user, CurrentUserService currentUserService) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var fridgeItems = await dbContext.FridgeItems
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Product)
                .Include(x => x.Unit)
                .ToListAsync();

            var items = fridgeItems.Select(x => new FridgeItemResponse
            {
                Id = x.Id,
                Name = x.Product.Name,
                Quantity = x.Quantity,
                Unit = x.Unit.ShortName
            }).ToList();

            return TypedResults.Ok(items);
        })
        .ProducesProblem(401);
    }
}
