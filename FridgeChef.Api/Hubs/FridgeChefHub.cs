using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Hubs;

public class FridgeChefHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            groupName);
    }
}