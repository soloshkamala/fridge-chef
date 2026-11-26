using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Services;

namespace FridgeChef.Api.Features.Recipes.Models;

public static class RecipeResponseMapper
{
    public static RecipeResponse Map(Recipe recipe, bool isEnglish)
    {
        return new RecipeResponse
        {
            Id = recipe.Id,
            Name = CatalogLocalizationService.Translate(
                recipe.Name, recipe.NameEn, isEnglish),
            Description = CatalogLocalizationService.Translate(
                recipe.Description, recipe.DescriptionEn, isEnglish),
            ImageUrl = recipe.ImageUrl,
            CategoryName = CatalogLocalizationService.Translate(
                recipe.Category.Name, recipe.Category.NameEn, isEnglish),
            CookingTime = recipe.CookingTime,
            Calories = recipe.Calories,
            Servings = recipe.Servings,
            Instructions = CatalogLocalizationService.Translate(
                recipe.Instructions, recipe.InstructionsEn, isEnglish),
            Ingredients = recipe.Ingredients
                .Select(ingredient => new RecipeIngredientResponse
                {
                    Name = CatalogLocalizationService.Translate(
                        ingredient.Product.Name,
                        ingredient.Product.NameEn,
                        isEnglish),
                    Quantity = ingredient.Quantity,
                    Unit = CatalogLocalizationService.Translate(
                        ingredient.Unit.ShortName,
                        ingredient.Unit.ShortNameEn,
                        isEnglish)
                })
                .ToList()
        };
    }
}