namespace FridgeChef.Models.Recipes;

public class RecipeIngredientResponse
{
    public string Name { get; set; } = "";

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = "";
}