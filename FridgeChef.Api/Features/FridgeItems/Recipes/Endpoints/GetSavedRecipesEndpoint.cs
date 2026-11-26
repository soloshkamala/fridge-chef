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
            HttpRequest request,
            ClaimsPrincipal user,
            CurrentUserService currentUserService,
            FridgeChefDbContext db,
            CancellationToken ct) =>
        {
            var userId = await currentUserService.GetUserIdAsync(user);

            if (userId is null)
                return Results.Unauthorized();

            var isEnglish = CatalogLocalizationService.IsEnglish(request);

            var recipes = await db.UserRecipeCollections
                .AsNoTracking()
                .Where(x => x.UserId == userId.Value && x.IsSaved)
                .Select(x => new
                {
                    Id = x.Recipe.Id.ToString(),
                    Title = x.Recipe.Name,
                    TitleEn = x.Recipe.NameEn,
                    ImageUrl = x.Recipe.ImageUrl,
                    Minutes = x.Recipe.CookingTime,
                    Kcal = x.Recipe.Calories
                })
                .ToListAsync(ct);

            var result = recipes.Select(recipe => new
            {
                recipe.Id,
                Title = CatalogLocalizationService.Translate(
                    recipe.Title, recipe.TitleEn, isEnglish),
                recipe.ImageUrl,
                recipe.Minutes,
                recipe.Kcal
            }).ToList();

            return Results.Ok(result);
        })
        .RequireAuthorization("ApiRead");
    }
}
