namespace FridgeChef.Api.Data.Entities;

public class ShoppingItem
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int UnitId { get; set; }

    public decimal Quantity { get; set; }

    public bool IsPurchased { get; set; }
    public bool IsInFridge { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
}