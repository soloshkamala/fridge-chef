
namespace FridgeChef.Services;

public interface IShoppingListSignalRService
{
    event Func<Task>? ShoppingListChanged;

    Task StartAsync();

    Task StopAsync();
}
