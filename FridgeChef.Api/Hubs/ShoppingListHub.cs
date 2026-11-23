
using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Hubs;

[Authorize(Policy = "ApiRead")]
public class ShoppingListHub : Hub
{
    private readonly CurrentUserService _currentUserService;

    public ShoppingListHub(CurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task JoinShoppingList()
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
            throw new HubException("User is not authenticated");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            ShoppingListHubGroups.ForUser(userId.Value));
    }

    public async Task LeaveShoppingList()
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
            throw new HubException("User is not authenticated");

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            ShoppingListHubGroups.ForUser(userId.Value));
    }
}

public static class ShoppingListHubGroups
{
    public static string ForUser(int userId)
        => $"shopping-list-user-{userId}";
}
