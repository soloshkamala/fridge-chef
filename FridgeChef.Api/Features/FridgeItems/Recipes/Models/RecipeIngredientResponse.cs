namespace FridgeChef.Api.Features.Recipes.Models;

public class RecipeIngredientResponse
{
    public string Name { get; set; } = "";

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = "";
}