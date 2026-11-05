using FridgeChef.Api.Features.FridgeItems;
using FridgeChef.Api.Features.Recipes.Models;

namespace FridgeChef.Api.Features.Recipes.Endpoints;

public static class GetRecommendedRecipesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/recommended", (
            FridgeItemsStore fridgeStore,
            RecipesStore recipesStore) =>
        {
            var fridgeItems = fridgeStore.Items;

            var result = recipesStore.Recipes.Select(recipe =>
            {
                var availableIngredients = recipe.Ingredients
                    .Where(ingredient => fridgeItems.Any(item =>
                        item.Name.Equals(
                            ingredient.Name,
                            StringComparison.OrdinalIgnoreCase)))
                    .Select(ingredient => ingredient.Name)
                    .ToList();

                var missingIngredients = recipe.Ingredients
                    .Where(ingredient => !fridgeItems.Any(item =>
                        item.Name.Equals(
                            ingredient.Name,
                            StringComparison.OrdinalIgnoreCase)))
                    .Select(ingredient => ingredient.Name)
                    .ToList();

                decimal matchPercentage = recipe.Ingredients.Count == 0
                    ? 0
                    : (decimal)availableIngredients.Count /
                      recipe.Ingredients.Count * 100;

                return new RecommendedRecipeResponse
                {
                    Recipe = recipe,
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