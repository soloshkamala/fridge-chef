
using FridgeChef.Api.Features.Recipes.Models;

namespace FridgeChef.Api.Features.Recipes;

public class RecipesStore
{
    public List<RecipeResponse> Recipes { get; } = new();
}