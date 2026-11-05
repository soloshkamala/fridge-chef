
namespace FridgeChef.Api.Data.Entities;

public class RecipeCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
