namespace FridgeChef.Api.Data.Entities;

public class UserRecipeCollection
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public bool IsSaved { get; set; }
    public bool IsFavorite { get; set; }
}