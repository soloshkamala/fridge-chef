using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Server.Tests;

public class ShoppingItemApiTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("fridgechef_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private ApiFactory factory = null!;
    private HttpClient client = null!;

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        factory = new ApiFactory
        {
            ConnectionString = postgres.GetConnectionString(),
            UseTestAuthentication = true
        };

        client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<FridgeChef.Api.Data.FridgeChefDbContext>();

        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        client.Dispose();
        await factory.DisposeAsync();
        await postgres.DisposeAsync();
    }

    [Fact]
    public async Task ShoppingItem_CreateGetUpdateDelete_Works()
    {
        // CREATE
        var createResponse = await client.PostAsJsonAsync(
            "/api/shopping-items",
            new
            {
                name = "Тестові яблука",
                quantity = 2,
                unit = "шт",
                category = "Тестові фрукти"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        Assert.NotNull(createResponse.Headers.Location);

        var location = createResponse.Headers.Location!;

        // GET одного запису
        var getResponse = await client.GetAsync(location);

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var item =
            await getResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Тестові яблука",
            item.GetProperty("name").GetString());

        Assert.Equal(
            2,
            item.GetProperty("quantity").GetDouble());

        // GET усіх записів
        var listResponse =
            await client.GetAsync("/api/shopping-items");

        Assert.Equal(
            HttpStatusCode.OK,
            listResponse.StatusCode);

        var id = item.GetProperty("id").GetInt32();

        // UPDATE
        var updateResponse = await client.PutAsJsonAsync(
            $"/api/shopping-items/{id}",
            new
            {
                name = "Зелені яблука",
                quantity = 5,
                unit = "шт",
                category = "Тестові фрукти",
                isPurchased = true,
                isInFridge = false
            });

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);

        var updatedResponse =
            await client.GetAsync($"/api/shopping-items/{id}");

        Assert.Equal(
            HttpStatusCode.OK,
            updatedResponse.StatusCode);

        var updatedItem =
            await updatedResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Зелені яблука",
            updatedItem.GetProperty("name").GetString());

        Assert.Equal(
            5,
            updatedItem.GetProperty("quantity").GetDouble());

        Assert.True(
            updatedItem.GetProperty("isPurchased").GetBoolean());

        // DELETE
        var deleteResponse =
            await client.DeleteAsync($"/api/shopping-items/{id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var afterDeleteResponse =
            await client.GetAsync($"/api/shopping-items/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            afterDeleteResponse.StatusCode);
    }
}
