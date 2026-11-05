
namespace FridgeChef.Api.Data.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public int ProductId { get; set; }

    public int UnitId { get; set; }

    public decimal Quantity { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public Unit Unit { get; set; } = null!;
}
