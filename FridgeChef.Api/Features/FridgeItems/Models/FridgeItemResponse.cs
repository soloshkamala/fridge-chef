namespace FridgeChef.Api.Features.FridgeItems.Models;

public class FridgeItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "";
}