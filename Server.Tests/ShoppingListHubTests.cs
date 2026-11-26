
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

public class ShoppingListHubTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("shopping_hub_test")
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
    public async Task ShoppingListHub_SendsEventsOnlyToUserGroup()
    {
        await using var firstConnection = CreateConnection("user1");
        await using var secondConnection = CreateConnection("user2");

        var firstReceived = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var secondReceived = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        firstConnection.On<int>(
            "ShoppingItemAdded",
            id => firstReceived.TrySetResult(id));

        secondConnection.On<int>(
            "ShoppingItemAdded",
            id => secondReceived.TrySetResult(id));

        await firstConnection.StartAsync();
        await secondConnection.StartAsync();

        await firstConnection.InvokeAsync("JoinShoppingList");
        await secondConnection.InvokeAsync("JoinShoppingList");

        var hubContext = factory.Services
            .GetRequiredService<IHubContext<ShoppingListHub>>();

        await hubContext.Clients
            .Group(ShoppingListHubGroups.ForUser(firstUserId))
            .SendAsync("ShoppingItemAdded", 123);

        var receivedId = await firstReceived.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        Assert.Equal(123, receivedId);

        await Task.Delay(500);

        Assert.False(
            secondReceived.Task.IsCompleted,
            "Another user must not receive this shopping list event.");
    }

    [Fact]
    public async Task ShoppingListHub_DoesNotSendEventsAfterLeavingGroup()
    {
        await using var connection = CreateConnection("user1");

        var received = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        connection.On<int>(
            "ShoppingItemAdded",
            id => received.TrySetResult(id));

        await connection.StartAsync();

        await connection.InvokeAsync("JoinShoppingList");

        await connection.InvokeAsync("LeaveShoppingList");

        var hubContext = factory.Services
            .GetRequiredService<IHubContext<ShoppingListHub>>();

        await hubContext.Clients
            .Group(ShoppingListHubGroups.ForUser(firstUserId))
            .SendAsync("ShoppingItemAdded", 456);

        await Task.Delay(500);

        Assert.False(
            received.Task.IsCompleted,
            "A user must not receive events after leaving the shopping list group.");
    }

    private HubConnection CreateConnection(string testUser)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                new Uri(
                    factory.Server.BaseAddress,
                    "/hubs/shopping-list"),
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
