using FridgeChef.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FridgeChef.Tests;

public class FridgeItemsApiTests : IClassFixture<FridgeChefWebApplicationFactory>
{
    private readonly HttpClient client;
    private readonly FridgeChefWebApplicationFactory factory;

    public FridgeItemsApiTests(FridgeChefWebApplicationFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    private async Task<int> CreateItem(
        string name = "Молоко",
        decimal quantity = 2,
        string unit = "л")
    {
        var response = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = name,
            Quantity = quantity,
            Unit = unit
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        return result.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task Can_Create_Get_Update_And_Delete_Item()
    {
        await factory.ResetFridgeItemsAsync();

        var createResponse = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = "Молоко",
            Quantity = 2,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        var getAllResponse = await client.GetAsync("/api/fridge-items");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/fridge-items/{id}", new
        {
            Name = "Молоко",
            Quantity = 3,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task Cannot_Create_Item_With_Invalid_Quantity()
    {
        await factory.ResetFridgeItemsAsync();

        var response = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = "Молоко",
            Quantity = 0,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Cannot_Create_Item_Without_Name()
    {
        await factory.ResetFridgeItemsAsync();

        var response = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = "",
            Quantity = 2,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Repeating_Post_Increases_Quantity()
    {
        await factory.ResetFridgeItemsAsync();

        var firstResponse = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = "Молоко",
            Quantity = 1,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync("/api/fridge-items", new
        {
            Name = "Молоко",
            Quantity = 500,
            Unit = "мл"
        });

        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);

        var result = await secondResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(1.5m, result.GetProperty("quantity").GetDecimal());
        Assert.Equal("л", result.GetProperty("unit").GetString());
    }

    [Fact]
    public async Task Getting_Missing_Item_Returns_NotFound()
    {
        await factory.ResetFridgeItemsAsync();

        var response = await client.GetAsync("/api/fridge-items/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Updating_Missing_Item_Returns_NotFound()
    {
        await factory.ResetFridgeItemsAsync();

        var response = await client.PutAsJsonAsync("/api/fridge-items/999999", new
        {
            Name = "Молоко",
            Quantity = 2,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Updating_With_Invalid_Quantity_Returns_BadRequest()
    {
        await factory.ResetFridgeItemsAsync();

        var id = await CreateItem();

        var response = await client.PutAsJsonAsync($"/api/fridge-items/{id}", new
        {
            Name = "Молоко",
            Quantity = 0,
            Unit = "л"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Updating_To_Existing_Item_Returns_Conflict()
    {
        await factory.ResetFridgeItemsAsync();

        var firstId = await CreateItem("Молоко", 2, "л");

        await CreateItem("Сир", 1, "кг");

        var response = await client.PutAsJsonAsync($"/api/fridge-items/{firstId}", new
        {
            Name = "Сир",
            Quantity = 2,
            Unit = "кг"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Repeating_Put_Returns_NoContent_Both_Times()
    {
        await factory.ResetFridgeItemsAsync();

        var id = await CreateItem();

        var request = new
        {
            Name = "Молоко",
            Quantity = 3,
            Unit = "л"
        };

        var firstResponse = await client.PutAsJsonAsync($"/api/fridge-items/{id}", request);
        var secondResponse = await client.PutAsJsonAsync($"/api/fridge-items/{id}", request);

        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Repeating_Delete_Returns_NotFound_The_Second_Time()
    {
        await factory.ResetFridgeItemsAsync();

        var id = await CreateItem();

        var firstResponse = await client.DeleteAsync($"/api/fridge-items/{id}");
        var secondResponse = await client.DeleteAsync($"/api/fridge-items/{id}");

        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, secondResponse.StatusCode);
    }

    [Fact]
    public async Task OpenApi_Document_Contains_All_Endpoints_And_Status_Codes()
    {
        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var paths = json.GetProperty("paths");

        Assert.True(paths.TryGetProperty("/api/fridge-items", out var collection));
        Assert.True(paths.TryGetProperty("/api/fridge-items/{id}", out var item));

        var getCollection = collection.GetProperty("get");
        var post = collection.GetProperty("post");
        var getItem = item.GetProperty("get");
        var put = item.GetProperty("put");
        var delete = item.GetProperty("delete");

        Assert.True(getCollection.GetProperty("responses").TryGetProperty("200", out _));

        Assert.True(post.GetProperty("responses").TryGetProperty("201", out _));
        Assert.True(post.GetProperty("responses").TryGetProperty("400", out _));
        Assert.True(post.GetProperty("responses").TryGetProperty("401", out _));

        Assert.True(getItem.GetProperty("responses").TryGetProperty("200", out _));
        Assert.True(getItem.GetProperty("responses").TryGetProperty("404", out _));

        Assert.True(put.GetProperty("responses").TryGetProperty("204", out _));
        Assert.True(put.GetProperty("responses").TryGetProperty("400", out _));
        Assert.True(put.GetProperty("responses").TryGetProperty("404", out _));
        Assert.True(put.GetProperty("responses").TryGetProperty("409", out _));

        Assert.True(delete.GetProperty("responses").TryGetProperty("204", out _));
        Assert.True(delete.GetProperty("responses").TryGetProperty("404", out _));
    }

    [Fact]
    public async Task Post_Creates_Item_And_Get_Returns_It_By_Location()
    {
        await factory.ResetFridgeItemsAsync();
        var postResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new
            {
                Name = "Молоко",
                Quantity = 2,
                Unit = "л"
            });

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var location = postResponse.Headers.Location;

        Assert.NotNull(location);

        var getResponse = await client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
}