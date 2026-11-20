
namespace FridgeChef.Api.Data.Entities;

public class ProductUnit
{
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int UnitId { get; set; }

    public Unit Unit { get; set; } = null!;
}
