using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using FridgeChef.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Hubs;

[Authorize(Policy = "ApiRead")]
public class FridgeChefHub : Hub
{
    private readonly CurrentUserService _currentUserService;
    private readonly FridgeChefDbContext _dbContext;

    public FridgeChefHub(
        CurrentUserService currentUserService,
        FridgeChefDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }
    public async Task JoinFridge()
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            throw new HubException("User is not authenticated");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            FridgeHubGroups.ForUser(userId.Value));
    }

    public async Task LeaveFridge()
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            throw new HubException("User is not authenticated");
        }

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            FridgeHubGroups.ForUser(userId.Value));
    }
    public async Task JoinItem(int itemId)
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            throw new HubException("User is not authenticated");
        }

        var itemExists = await _dbContext.FridgeItems
            .AnyAsync(x =>
                x.Id == itemId &&
                x.UserId == userId.Value);

        if (!itemExists)
        {
            throw new HubException("Fridge item not found");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            FridgeHubGroups.ForItem(userId.Value, itemId));
    }

    public async Task LeaveItem(int itemId)
    {
        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            throw new HubException("User is not authenticated");
        }

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            FridgeHubGroups.ForItem(userId.Value, itemId));
    }
}