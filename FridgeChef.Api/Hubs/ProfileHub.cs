
using System.Diagnostics;
using FridgeChef.Api.Observability;
using FridgeChef.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FridgeChef.Api.Hubs;

[Authorize(Policy = "ApiRead")]
public class ProfileHub : Hub
{
    private readonly CurrentUserService _currentUserService;
    private readonly ILogger<ProfileHub> _logger;

    public ProfileHub(
        CurrentUserService currentUserService,
        ILogger<ProfileHub> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task JoinProfile()
    {
        using var activity =
            FridgeTelemetry.ActivitySource.StartActivity(
                "Profile.Join");

        activity?.SetTag("signalr.connection_id", Context.ConnectionId);

        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                "User is not authenticated");

            _logger.LogWarning(
                "Profile subscription failed. ConnectionId: {ConnectionId}",
                Context.ConnectionId);

            throw new HubException("User is not authenticated");
        }

        activity?.SetTag("user.id", userId.Value);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            ProfileHubGroups.ForUser(userId.Value));

        _logger.LogInformation(
            "User {UserId} subscribed to profile updates. ConnectionId: {ConnectionId}",
            userId.Value,
            Context.ConnectionId);
    }

    public async Task LeaveProfile()
    {
        using var activity =
            FridgeTelemetry.ActivitySource.StartActivity(
                "Profile.Leave");

        activity?.SetTag("signalr.connection_id", Context.ConnectionId);

        var userId = await _currentUserService
            .GetUserIdAsync(Context.User!);

        if (userId == null)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                "User is not authenticated");

            _logger.LogWarning(
                "Profile unsubscription failed. ConnectionId: {ConnectionId}",
                Context.ConnectionId);

            throw new HubException("User is not authenticated");
        }

        activity?.SetTag("user.id", userId.Value);

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            ProfileHubGroups.ForUser(userId.Value));

        _logger.LogInformation(
            "User {UserId} unsubscribed from profile updates. ConnectionId: {ConnectionId}",
            userId.Value,
            Context.ConnectionId);
    }
}

public static class ProfileHubGroups
{
    public static string ForUser(int userId)
        => $"profile-user-{userId}";
}
