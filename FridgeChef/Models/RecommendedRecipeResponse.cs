namespace FridgeChef.Models.Recipes;

public class RecommendedRecipeResponse
{
    public RecipeResponse Recipe { get; set; } = new();

    public List<string> AvailableIngredients { get; set; } = new();

    public List<string> MissingIngredients { get; set; } = new();

    public decimal MatchPercentage { get; set; }
}