using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FridgeChef.Api.Data;
using FridgeChef.Api.Features.Recipes.Models;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class GetRecommendedRecipesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
            group.MapGet("/recommended", async (
               FridgeChefDbContext dbContext,
               ClaimsPrincipal user,
               CurrentUserService currentUserService,
               CancellationToken cancellationToken) =>
            {
                var userId = await currentUserService.GetUserIdAsync(user);

                if (userId == null)
                {
                    return Results.Unauthorized();
                }

                var fridgeProductIds = await dbContext.FridgeItems
                .Where(item => item.UserId == userId.Value)
                .Select(item => item.ProductId)
                .ToListAsync(cancellationToken);

            var fridgeProductIdSet = fridgeProductIds.ToHashSet();

            var recipes = await dbContext.Recipes
                .AsNoTracking()
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ingredient => ingredient.Product)
                .Include(recipe => recipe.Ingredients)
                    .ThenInclude(ingredient => ingredient.Unit)
                .ToListAsync(cancellationToken);

            var result = recipes.Select(recipe =>
            {
                var requiredIngredients = recipe.Ingredients
                    .Where(ingredient => !ingredient.Product.IsPantryStaple)
                    .ToList();

                var availableIngredients = requiredIngredients
                    .Where(ingredient =>
                        fridgeProductIdSet.Contains(ingredient.ProductId))
                    .Select(ingredient => ingredient.Product.Name)
                    .ToList();

                var missingIngredients = requiredIngredients
                    .Where(ingredient =>
                        !fridgeProductIdSet.Contains(ingredient.ProductId))
                    .Select(ingredient => ingredient.Product.Name)
                    .ToList();

                decimal matchPercentage = requiredIngredients.Count == 0
                    ? 0
                    : (decimal)availableIngredients.Count /
                      requiredIngredients.Count * 100;

                return new RecommendedRecipeResponse
                {
                    Recipe = new RecipeResponse
                    {
                        Id = recipe.Id,
                        Name = recipe.Name,
                        Description = recipe.Description,
                        ImageUrl = recipe.ImageUrl,
                        CookingTime = recipe.CookingTime,
                        Calories = recipe.Calories,
                        Servings = recipe.Servings,
                        Instructions = recipe.Instructions,

                        Ingredients = recipe.Ingredients.Select(ingredient =>
                            new RecipeIngredientResponse
                            {
                                Name = ingredient.Product.Name,
                                Quantity = ingredient.Quantity,
                                Unit = ingredient.Unit.ShortName
                            }).ToList()
                    },
                    AvailableIngredients = availableIngredients,
                    MissingIngredients = missingIngredients,
                    MatchPercentage = Math.Round(matchPercentage, 2)
                };
            })
            .Where(recipe => recipe.MatchPercentage > 0)
            .OrderByDescending(recipe => recipe.MatchPercentage)
            .ToList();

            return Results.Ok(result);
        });
    }
}

