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
            HttpRequest request,
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

            var isEnglish = CatalogLocalizationService.IsEnglish(request);

            var fridgeProductIds = await dbContext.FridgeItems
                .Where(item => item.UserId == userId.Value)
                .Select(item => item.ProductId)
                .ToListAsync(cancellationToken);

            var fridgeProductIdSet = fridgeProductIds.ToHashSet();

            var recipes = await dbContext.Recipes
                .AsNoTracking()
                .Include(recipe => recipe.Category)
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
                    .Select(ingredient =>
                        CatalogLocalizationService.Translate(
                            ingredient.Product.Name,
                            ingredient.Product.NameEn,
                            isEnglish))
                    .ToList();

                var missingIngredients = requiredIngredients
                    .Where(ingredient =>
                        !fridgeProductIdSet.Contains(ingredient.ProductId))
                    .Select(ingredient =>
                        CatalogLocalizationService.Translate(
                            ingredient.Product.Name,
                            ingredient.Product.NameEn,
                            isEnglish))
                    .ToList();

                decimal matchPercentage = requiredIngredients.Count == 0
                    ? 0
                    : (decimal)availableIngredients.Count /
                      requiredIngredients.Count * 100;

                return new RecommendedRecipeResponse
                {
                    Recipe = RecipeResponseMapper.Map(recipe, isEnglish),
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
