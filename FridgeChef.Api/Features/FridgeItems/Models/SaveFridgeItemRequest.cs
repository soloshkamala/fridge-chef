namespace FridgeChef.Api.Features.FridgeItems.Models;

public class SaveFridgeItemRequest
{
    public string Name { get; set; } = "";
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "";
}