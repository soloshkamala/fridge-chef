using FridgeChef.Api.Data;
using FridgeChef.Api.Features.Recipes.Models;
using FridgeChef.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class GetRecipeByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            HttpRequest request,
            FridgeChefDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var isEnglish = CatalogLocalizationService.IsEnglish(request);

            var recipe = await dbContext.Recipes
                .AsNoTracking()
                .Include(r => r.Category)
                .Include(r => r.Ingredients)
                    .ThenInclude(i => i.Product)
                .Include(r => r.Ingredients)
                    .ThenInclude(i => i.Unit)
                .FirstOrDefaultAsync(
                    r => r.Id == id,
                    cancellationToken);

            if (recipe == null)
            {
                return Results.NotFound();
            }

            var result = RecipeResponseMapper.Map(recipe, isEnglish);

            return Results.Ok(result);
        });
    }
}
