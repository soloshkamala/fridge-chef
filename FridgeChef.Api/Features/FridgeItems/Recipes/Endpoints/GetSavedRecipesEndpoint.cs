using System.Security.Claims;
using FridgeChef.Api.Data;
using FridgeChef.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class GetSavedRecipesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/saved", async (
            ClaimsPrincipal user,
            CurrentUserService currentUserService,
            FridgeChefDbContext db,
            CancellationToken ct) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId is null)
                return Results.Unauthorized();

            var recipes = await db.UserRecipeCollections
                .AsNoTracking()
                .Where(x => x.UserId == userId.Value && x.IsSaved)
                .Select(x => new
                {
                    Id = x.Recipe.Id.ToString(),
                    Title = x.Recipe.Name,
                    ImageUrl = x.Recipe.ImageUrl,
                    Minutes = x.Recipe.CookingTime,
                    Kcal = x.Recipe.Calories
                })
                .ToListAsync(ct);

            return Results.Ok(recipes);
        })
        .RequireAuthorization("ApiRead");
    }
}
