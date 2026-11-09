namespace FridgeChef.Api.Data.Entities;

public class Recipe
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public string ImageUrl { get; set; } = "";

    public int CookingTime { get; set; }

    public int Calories { get; set; }

    public int Servings { get; set; }

    public string Instructions { get; set; } = "";

    public RecipeCategory Category { get; set; } = null!;

    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}