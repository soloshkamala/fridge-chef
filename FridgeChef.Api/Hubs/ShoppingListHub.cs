
using System.Diagnostics;
using FridgeChef.Api.Observability;
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
        using var activity =
            FridgeTelemetry.ActivitySource.StartActivity(
                "ShoppingList.Join");

        activity?.SetTag("signalr.connection_id", Context.ConnectionId);

        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                "User is not authenticated");

            throw new HubException("User is not authenticated");
        }

        activity?.SetTag("user.id", userId.Value);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            ShoppingListHubGroups.ForUser(userId.Value));
    }

    public async Task LeaveShoppingList()
    {
        using var activity =
            FridgeTelemetry.ActivitySource.StartActivity(
                "ShoppingList.Leave");

        activity?.SetTag("signalr.connection_id", Context.ConnectionId);

        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                "User is not authenticated");

            throw new HubException("User is not authenticated");
        }

        activity?.SetTag("user.id", userId.Value);

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
