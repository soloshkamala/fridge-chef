
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FridgeChef.Tests;

public class FridgeItemsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public FridgeItemsApiTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Can_Create_Get_Update_And_Delete_Item()
    {
        var name = "Milk" + Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 2, Unit = "liters" });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var json = await createResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("id").GetInt32();

        var getAllResponse = await client.GetAsync("/api/fridge-items");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/fridge-items/{id}",
            new { Name = name, Quantity = 3, Unit = "liters" });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync($"/api/fridge-items/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);

        await CheckProblemDetails(missingResponse, 404);
    }

    [Fact]
    public async Task Cannot_Create_Item_With_Invalid_Quantity()
    {
        var response = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = "Invalid" + Guid.NewGuid(), Quantity = 0, Unit = "liters" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await CheckProblemDetails(response, 400);
    }

    [Fact]
    public async Task Cannot_Create_Item_Without_Name()
    {
        var response = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = "", Quantity = 2, Unit = "liters" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await CheckProblemDetails(response, 400);
    }

    [Fact]
    public async Task Cannot_Create_Duplicate_Item()
    {
        var name = "Duplicate" + Guid.NewGuid();

        var firstResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 1, Unit = "kg" });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 2, Unit = "kg" });

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        await CheckProblemDetails(secondResponse, 409);
    }

    [Fact]
    public async Task Getting_Missing_Item_Returns_NotFound()
    {
        var response = await client.GetAsync("/api/fridge-items/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await CheckProblemDetails(response, 404);
    }

    [Fact]
    public async Task Updating_Missing_Item_Returns_NotFound()
    {
        var response = await client.PutAsJsonAsync(
            "/api/fridge-items/999999",
            new { Name = "Milk", Quantity = 2, Unit = "liters" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await CheckProblemDetails(response, 404);
    }

    [Fact]
    public async Task Updating_With_Invalid_Quantity_Returns_BadRequest()
    {
        var name = "Update" + Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 2, Unit = "kg" });

        var json = await createResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("id").GetInt32();

        var response = await client.PutAsJsonAsync(
            $"/api/fridge-items/{id}",
            new { Name = name, Quantity = 0, Unit = "kg" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await CheckProblemDetails(response, 400);
    }

    [Fact]
    public async Task Updating_To_Existing_Item_Returns_Conflict()
    {
        var suffix = Guid.NewGuid().ToString();

        var firstResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = "First" + suffix, Quantity = 1, Unit = "kg" });

        var secondResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = "Second" + suffix, Quantity = 2, Unit = "kg" });

        var firstJson = await firstResponse.Content.ReadAsStringAsync();
        using var firstDocument = JsonDocument.Parse(firstJson);
        var firstId = firstDocument.RootElement.GetProperty("id").GetInt32();

        var secondJson = await secondResponse.Content.ReadAsStringAsync();
        using var secondDocument = JsonDocument.Parse(secondJson);
        var secondName = secondDocument.RootElement.GetProperty("name").GetString();

        var response = await client.PutAsJsonAsync(
            $"/api/fridge-items/{firstId}",
            new { Name = secondName, Quantity = 3, Unit = "kg" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await CheckProblemDetails(response, 409);
    }

    [Fact]
    public async Task Repeating_Put_Returns_NoContent_Both_Times()
    {
        var name = "RepeatPut" + Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 1, Unit = "kg" });

        var json = await createResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("id").GetInt32();

        var request = new { Name = name, Quantity = 2, Unit = "kg" };

        var firstPut = await client.PutAsJsonAsync(
            $"/api/fridge-items/{id}", request);

        var secondPut = await client.PutAsJsonAsync(
            $"/api/fridge-items/{id}", request);

        Assert.Equal(HttpStatusCode.NoContent, firstPut.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondPut.StatusCode);
    }

    [Fact]
    public async Task Repeating_Delete_Returns_NotFound_The_Second_Time()
    {
        var name = "RepeatDelete" + Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync(
            "/api/fridge-items",
            new { Name = name, Quantity = 1, Unit = "kg" });

        var json = await createResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("id").GetInt32();

        var firstDelete = await client.DeleteAsync($"/api/fridge-items/{id}");
        var secondDelete = await client.DeleteAsync($"/api/fridge-items/{id}");

        Assert.Equal(HttpStatusCode.NoContent, firstDelete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);

        await CheckProblemDetails(secondDelete, 404);
    }

    [Fact]
    public async Task OpenApi_Document_Contains_All_Endpoints_And_Status_Codes()
    {
        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        var paths = document.RootElement.GetProperty("paths");

        Assert.True(paths.TryGetProperty("/api/fridge-items", out var collection));
        Assert.True(paths.TryGetProperty("/api/fridge-items/{id}", out var item));

        Assert.True(collection.GetProperty("get").GetProperty("responses")
            .TryGetProperty("200", out _));

        var postResponses = collection.GetProperty("post").GetProperty("responses");
        Assert.True(postResponses.TryGetProperty("201", out _));
        Assert.True(postResponses.TryGetProperty("400", out _));
        Assert.True(postResponses.TryGetProperty("409", out _));

        var getResponses = item.GetProperty("get").GetProperty("responses");
        Assert.True(getResponses.TryGetProperty("200", out _));
        Assert.True(getResponses.TryGetProperty("404", out _));

        var putResponses = item.GetProperty("put").GetProperty("responses");
        Assert.True(putResponses.TryGetProperty("204", out _));
        Assert.True(putResponses.TryGetProperty("400", out _));
        Assert.True(putResponses.TryGetProperty("404", out _));
        Assert.True(putResponses.TryGetProperty("409", out _));

        var deleteResponses = item.GetProperty("delete").GetProperty("responses");
        Assert.True(deleteResponses.TryGetProperty("204", out _));
        Assert.True(deleteResponses.TryGetProperty("404", out _));
    }

    private static async Task CheckProblemDetails(
        HttpResponseMessage response,
        int expectedStatus)
    {
        Assert.Contains(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType ?? "");

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        Assert.Equal(
            expectedStatus,
            document.RootElement.GetProperty("status").GetInt32());

        Assert.True(
            document.RootElement.TryGetProperty("title", out _));
    }
}
