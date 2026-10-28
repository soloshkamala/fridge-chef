using FridgeChef.Api.Features.FridgeItems.Models;

namespace FridgeChef.Api.Features.FridgeItems;

public class FridgeItemsStore
{
    public List<FridgeItemResponse> Items { get; } = new();

    public int NextId { get; set; } = 1;
}