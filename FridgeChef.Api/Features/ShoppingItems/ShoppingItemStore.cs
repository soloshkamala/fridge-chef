
namespace FridgeChef.Api.Features.ShoppingItems;

public class ShoppingItemStore
{
    private readonly List<ShoppingItemResponse> _items = new()
    {
        new(1, "Помідори", 2, "кг", "Овочі та фрукти", false, false),
        new(2, "Огірки", 1, "кг", "Овочі та фрукти", false, false),
        new(3, "Яблука", 5, "шт.", "Овочі та фрукти", false, false),
        new(4, "Куряче філе", 1, "кг", "М'ясо та птиця", false, false),
        new(5, "Індичка", 0.5, "кг", "М'ясо та птиця", false, false)
    };

    private int _nextId = 6;

    public List<ShoppingItemResponse> GetAll() => _items.ToList();

    public ShoppingItemResponse? GetById(int id) =>
        _items.FirstOrDefault(item => item.Id == id);

    public ShoppingItemResponse Add(CreateShoppingItemRequest request)
    {
        var item = new ShoppingItemResponse(
            _nextId++,
            request.Name.Trim(),
            request.Quantity,
            request.Unit.Trim(),
            request.Category.Trim(),
            false,
            false);

        _items.Add(item);
        return item;
    }

    public ShoppingItemResponse? Update(
        int id,
        UpdateShoppingItemRequest request)
    {
        var index = _items.FindIndex(item => item.Id == id);

        if (index == -1)
            return null;

        var item = new ShoppingItemResponse(
            id,
            request.Name.Trim(),
            request.Quantity,
            request.Unit.Trim(),
            request.Category.Trim(),
            request.IsPurchased,
            request.IsInFridge);

        _items[index] = item;
        return item;
    }

    public bool Delete(int id)
    {
        var item = GetById(id);

        if (item is null)
            return false;

        _items.Remove(item);
        return true;
    }
}
