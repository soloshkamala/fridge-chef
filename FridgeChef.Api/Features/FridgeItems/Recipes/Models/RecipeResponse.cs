
namespace FridgeChef.Api.Features.Recipes.Models;

public class RecipeResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public string ImageUrl { get; set; } = "";
    public int CookingTime { get; set; }

    public int Calories { get; set; }

    public int Servings { get; set; }

    public string Instructions { get; set; } = "";


    public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
}