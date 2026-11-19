using FridgeChef.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.FridgeItems.Endpoints;

public static class DeleteFridgeItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async Task<Results<NoContent, ProblemHttpResult>>
            (int id, FridgeChefDbContext dbContext,ClaimsPrincipal user, CurrentUserService currentUserService) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId == null)
            {
                return TypedResults.Problem(
                    statusCode: 401,
                    title: "User is not authenticated");
            }

            var item = await dbContext.FridgeItems
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.UserId == userId.Value);

            if (item == null)
            {
                return TypedResults.Problem(
                    statusCode: 404,
                    title: "Product not found");
            }

            dbContext.FridgeItems.Remove(item);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .ProducesProblem(401)
        .ProducesProblem(404);
    }
}