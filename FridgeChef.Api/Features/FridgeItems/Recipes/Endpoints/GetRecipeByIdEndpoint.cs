
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.Recipes.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class GetRecipeByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            FridgeChefDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
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

            var result = new RecipeResponse
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                CategoryName = recipe.Category.Name,
                CookingTime = recipe.CookingTime,
                Calories = recipe.Calories,
                Servings = recipe.Servings,
                Instructions = recipe.Instructions,

                Ingredients = recipe.Ingredients
                    .Select(i => new RecipeIngredientResponse
                    {
                        Name = i.Product.Name,
                        Quantity = i.Quantity,
                        Unit = i.Unit.ShortName
                    })
                    .ToList()
            };

            return Results.Ok(result);
        });
    }
}
