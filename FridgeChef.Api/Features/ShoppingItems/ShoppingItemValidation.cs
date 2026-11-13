
namespace FridgeChef.Api.Features.ShoppingItems;

public static class ShoppingItemValidation
{
    public static bool IsValid(
        string? name,
        double quantity,
        string? unit,
        string? category)
    {
        return !string.IsNullOrWhiteSpace(name)
            && name.Trim().Length <= 100
            && !string.IsNullOrWhiteSpace(unit)
            && unit.Trim().Length <= 30
            && !string.IsNullOrWhiteSpace(category)
            && category.Trim().Length <= 50
            && !double.IsNaN(quantity)
            && !double.IsInfinity(quantity)
            && quantity >= 0.01
            && quantity <= 1000;
    }
}
