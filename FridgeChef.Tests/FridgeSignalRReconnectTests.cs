using FridgeChef.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Moq;
using System.Reflection;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace FridgeChef.Tests;

public class FridgeSignalRReconnectTests
{
    private static FridgeSignalRService CreateService()
    {
        var tokenProvider = new Mock<IAccessTokenProvider>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Api:BaseAddress"] = "http://localhost:5039/"
            })
            .Build();

        return new FridgeSignalRService(
            tokenProvider.Object,
            configuration);
    }

    private static void SetSubscribedItems(
        FridgeSignalRService service,
        params int[] itemIds)
    {
        var field = typeof(FridgeSignalRService).GetField(
            "_subscribedItemIds",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);

        var subscribedIds = Assert.IsType<HashSet<int>>(
            field.GetValue(service));

        subscribedIds.Clear();
        subscribedIds.UnionWith(itemIds);
    }

    private static async Task<List<(string Method, int? ItemId)>>
        GetInvocationsAsync(FridgeSignalRService service)
    {
        var invocations = new List<(string Method, int? ItemId)>();

        await service.JoinGroupsAsync((methodName, itemId) =>
        {
            invocations.Add((methodName, itemId));

            return Task.CompletedTask;
        });

        return invocations;
    }

    [Fact]
    public async Task Reconnected_Rejoins_Fridge_Group()
    {
        var service = CreateService();

        var invocations = await GetInvocationsAsync(service);

        Assert.Single(invocations);
        Assert.Equal("JoinFridge", invocations[0].Method);
        Assert.Null(invocations[0].ItemId);
    }

    [Fact]
    public async Task Reconnected_Rejoins_All_Subscribed_Items()
    {
        var service = CreateService();

        SetSubscribedItems(service, 1, 2, 3);

        var invocations = await GetInvocationsAsync(service);

        Assert.Equal(4, invocations.Count);

        Assert.Contains(invocations,
            x => x.Method == "JoinFridge");

        foreach (var itemId in new[] { 1, 2, 3 })
        {
            Assert.Contains(invocations,
                x => x.Method == "JoinItem"
                     && x.ItemId == itemId);
        }
    }

    [Fact]
    public async Task Reconnected_Without_Items_Joins_Only_Fridge()
    {
        var service = CreateService();

        SetSubscribedItems(service);

        var invocations = await GetInvocationsAsync(service);

        Assert.Single(invocations);

        Assert.DoesNotContain(invocations,
            x => x.Method == "JoinItem");
    }

    [Fact]
    public async Task Reconnected_Passes_Correct_ItemIds()
    {
        var service = CreateService();

        SetSubscribedItems(service, 10, 25, 50);

        var invocations = await GetInvocationsAsync(service);

        var joinedItemIds = invocations
            .Where(x => x.Method == "JoinItem")
            .Select(x => x.ItemId)
            .ToHashSet();

        Assert.True(
            joinedItemIds.SetEquals(new int?[] { 10, 25, 50 }));
    }

    [Fact]
    public async Task Reconnected_Does_Not_Join_Duplicate_Items()
    {
        var service = CreateService();

        SetSubscribedItems(service, 5, 5, 10, 10);

        var invocations = await GetInvocationsAsync(service);

        var itemInvocations = invocations
            .Where(x => x.Method == "JoinItem")
            .ToList();

        Assert.Equal(2, itemInvocations.Count);

        Assert.Single(itemInvocations,
            x => x.ItemId == 5);

        Assert.Single(itemInvocations,
            x => x.ItemId == 10);
    }

    [Fact]
    public async Task Reconnected_Joins_Fridge_Before_Items()
    {
        var service = CreateService();

        SetSubscribedItems(service, 1, 2);

        var invocations = await GetInvocationsAsync(service);

        Assert.Equal("JoinFridge", invocations[0].Method);

        Assert.All(invocations.Skip(1),
            x => Assert.Equal("JoinItem", x.Method));
    }

    [Fact]
    public async Task Reconnected_Can_Rejoin_Groups_Multiple_Times()
    {
        var service = CreateService();

        SetSubscribedItems(service, 1, 2);

        var firstInvocations = await GetInvocationsAsync(service);
        var secondInvocations = await GetInvocationsAsync(service);

        Assert.Equal(firstInvocations, secondInvocations);

        Assert.Equal(3, firstInvocations.Count);
        Assert.Equal(3, secondInvocations.Count);
    }

    [Fact]
    public async Task Reconnected_Handler_Without_Connection_Does_Not_Throw()
    {
        var service = CreateService();

        var method = typeof(FridgeSignalRService).GetMethod(
            "HandleReconnectedAsync",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(method);

        var task = Assert.IsAssignableFrom<Task>(
            method.Invoke(service, new object?[] { "new-connection-id" }));

        await task;
    }
}