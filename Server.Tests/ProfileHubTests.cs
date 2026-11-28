
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Server.Tests;

public class ProfileHubTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("profile_hub_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApiFactory factory = null!;
    private int firstUserId;

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        factory = new ApiFactory
        {
            ConnectionString = postgres.GetConnectionString(),
            UseTestAuthentication = true
        };

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        await db.Database.MigrateAsync();

        var firstUser = new User
        {
            Email = "shopping-test@example.com",
            FirstName = "First",
            LastName = "User",
            EntraObjectId = "11111111-1111-1111-1111-111111111111",
            EntraTenantId = "22222222-2222-2222-2222-222222222222"
        };

        var secondUser = new User
        {
            Email = "shopping-test2@example.com",
            FirstName = "Second",
            LastName = "User",
            EntraObjectId = "33333333-3333-3333-3333-333333333333",
            EntraTenantId = "22222222-2222-2222-2222-222222222222"
        };

        db.Users.AddRange(firstUser, secondUser);
        await db.SaveChangesAsync();

        firstUserId = firstUser.Id;
    }

    public async Task DisposeAsync()
    {
        await factory.DisposeAsync();
        await postgres.DisposeAsync();
    }

    [Fact]
    public async Task ProfileHub_SendsEventsOnlyToProfileOwner()
    {
        await using var firstConnection = CreateConnection("user1");
        await using var secondConnection = CreateConnection("user2");

        var firstReceived = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var secondReceived = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        firstConnection.On(
            "ProfileChanged",
            () => firstReceived.TrySetResult(true));

        secondConnection.On(
            "ProfileChanged",
            () => secondReceived.TrySetResult(true));

        await firstConnection.StartAsync();
        await secondConnection.StartAsync();

        await firstConnection.InvokeAsync("JoinProfile");
        await secondConnection.InvokeAsync("JoinProfile");

        var hubContext = factory.Services
            .GetRequiredService<IHubContext<ProfileHub>>();

        await hubContext.Clients
            .Group(ProfileHubGroups.ForUser(firstUserId))
            .SendAsync("ProfileChanged");

        var eventReceived = await firstReceived.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        Assert.True(eventReceived);

        await Task.Delay(500);

        Assert.False(
            secondReceived.Task.IsCompleted,
            "Another user must not receive this profile event.");
    }

    [Fact]
    public async Task ProfileHub_DoesNotSendEventsAfterLeavingGroup()
    {
        await using var connection = CreateConnection("user1");

        var received = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        connection.On(
            "ProfileChanged",
            () => received.TrySetResult(1));

        await connection.StartAsync();

        await connection.InvokeAsync("JoinProfile");
        await connection.InvokeAsync("LeaveProfile");

        var hubContext = factory.Services
            .GetRequiredService<IHubContext<ProfileHub>>();

        await hubContext.Clients
            .Group(ProfileHubGroups.ForUser(firstUserId))
            .SendAsync("ProfileChanged");

        await Task.Delay(500);

        Assert.False(
            received.Task.IsCompleted,
            "A user must not receive events after leaving the profile group.");
    }

    [Fact]
    public async Task ProfileHub_RejoinsGroupAfterReconnection()
    {
        await using var connection = CreateConnection("user1");

        var received = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        connection.On(
            "ProfileChanged",
            () => received.TrySetResult(true));

        await connection.StartAsync();

        await connection.InvokeAsync("JoinProfile");

        
        await connection.StopAsync();
        await connection.StartAsync();

        
        await connection.InvokeAsync("JoinProfile");

        var hubContext = factory.Services
            .GetRequiredService<IHubContext<ProfileHub>>();

        await hubContext.Clients
            .Group(ProfileHubGroups.ForUser(firstUserId))
            .SendAsync("ProfileChanged");

        var eventReceived = await received.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        Assert.True(eventReceived);
    }

    private HubConnection CreateConnection(string testUser)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                new Uri(
                    factory.Server.BaseAddress,
                    "/hubs/profile"),
                options =>
                {
                    options.Transports = HttpTransportType.LongPolling;

                    options.HttpMessageHandlerFactory =
                        _ => factory.Server.CreateHandler();

                    options.Headers["X-Test-User"] = testUser;
                })
            .Build();
    }
}

