
using FridgeChef.Tests.Helpers;
using FridgeChef.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FridgeChef.Tests;

public class FridgeSignalRTests
{
    [Fact]
    public void ForUser_Returns_Correct_Group_Name()
    {
        var groupName = FridgeHubGroups.ForUser(1);

        Assert.Equal("fridge-user-1", groupName);
    }

    [Fact]
    public void ForItem_Returns_Correct_Group_Name()
    {
        var groupName = FridgeHubGroups.ForItem(1, 5);

        Assert.Equal("fridge-item-1-5", groupName);
    }

    [Fact]
    public void Different_Users_Have_Different_Item_Groups()
    {
        var firstGroup = FridgeHubGroups.ForItem(1, 5);
        var secondGroup = FridgeHubGroups.ForItem(2, 5);

        Assert.NotEqual(firstGroup, secondGroup);
    }
}

public class FridgeSignalRBroadcastTests
    : IClassFixture<FridgeSignalRWebApplicationFactory>
{
    private readonly FridgeSignalRWebApplicationFactory factory;
    private readonly HttpClient client;

    public FridgeSignalRBroadcastTests(
        FridgeSignalRWebApplicationFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Creating_Item_Sends_SignalR_Event_To_Correct_Groups()
    {
        await factory.ResetFridgeItemsAsync();

        factory.HubClientsMock.Invocations.Clear();
        factory.ClientProxyMock.Invocations.Clear();

        var response = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new
            {
                Name = "Молоко",
                Quantity = 2,
                Unit = "л"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var itemId = created.GetProperty("id").GetInt32();

        factory.HubClientsMock.Verify(
            x => x.Groups(
                It.Is<string[]>(groups =>
                    groups.Contains(
                        FridgeHubGroups.ForItem(1, itemId)) &&
                    groups.Contains(
                        FridgeHubGroups.ForUser(1)) &&
                    groups.Length == 2)),
            Times.Once);

        factory.ClientProxyMock.Verify(
            x => x.SendCoreAsync(
                "FridgeItemChanged",
                It.Is<object?[]>(args =>
                args.Length == 1 &&
                args[0] is int &&
                (int)args[0]! == itemId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Updating_Item_Sends_SignalR_Event_To_Correct_Groups()
    {
        await factory.ResetFridgeItemsAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new
            {
                Name = "Молоко",
                Quantity = 2,
                Unit = "л"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var itemId = created.GetProperty("id").GetInt32();

        factory.HubClientsMock.Invocations.Clear();
        factory.ClientProxyMock.Invocations.Clear();

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/fridge-items/{itemId}",
            new
            {
                Name = "Молоко",
                Quantity = 5,
                Unit = "л"
            });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        factory.HubClientsMock.Verify(
            x => x.Groups(
                It.Is<string[]>(groups =>
                    groups.Contains(
                        FridgeHubGroups.ForItem(1, itemId)) &&
                    groups.Contains(
                        FridgeHubGroups.ForUser(1)) &&
                    groups.Length == 2)),
            Times.Once);

        factory.ClientProxyMock.Verify(
            x => x.SendCoreAsync(
                "FridgeItemChanged",
                It.Is<object?[]>(args =>
                args.Length == 1 &&
                args[0] is int &&
                (int)args[0]! == itemId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Deleting_Item_Sends_SignalR_Event_To_Correct_Groups()
    {
        await factory.ResetFridgeItemsAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new
            {
                Name = "Молоко",
                Quantity = 2,
                Unit = "л"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var itemId = created.GetProperty("id").GetInt32();

        factory.HubClientsMock.Invocations.Clear();
        factory.ClientProxyMock.Invocations.Clear();

        var deleteResponse = await client.DeleteAsync(
            $"/api/fridge-items/{itemId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        factory.HubClientsMock.Verify(
            x => x.Groups(
                It.Is<string[]>(groups =>
                    groups.Contains(
                        FridgeHubGroups.ForItem(1, itemId)) &&
                    groups.Contains(
                        FridgeHubGroups.ForUser(1)) &&
                    groups.Length == 2)),
            Times.Once);

        factory.ClientProxyMock.Verify(
            x => x.SendCoreAsync(
                "FridgeItemChanged",
               It.Is<object?[]>(args =>
                args.Length == 1 &&
                args[0] is int &&
                (int)args[0]! == itemId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


}
