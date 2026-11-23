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

        db.Users.Add(new FridgeChef.Api.Data.Entities.User
        {
            Email = "shopping-test@example.com",
            FirstName = "Test",
            LastName = "User",
            EntraObjectId = "11111111-1111-1111-1111-111111111111",
            EntraTenantId = "22222222-2222-2222-2222-222222222222"
        });

        await db.SaveChangesAsync();

        var category = new FridgeChef.Api.Data.Entities.Category
        {
            Name = "Тестові фрукти"
        };

        var unit = new FridgeChef.Api.Data.Entities.Unit
        {
            Name = "Тестові штуки",
            ShortName = "тшт"
        };

        db.Categories.Add(category);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var firstProduct = new FridgeChef.Api.Data.Entities.Product
        {
            Name = "Тестові яблука",
            CategoryId = category.Id
        };

        var secondProduct = new FridgeChef.Api.Data.Entities.Product
        {
            Name = "Зелені яблука",
            CategoryId = category.Id
        };

        db.Products.AddRange(firstProduct, secondProduct);
        await db.SaveChangesAsync();

        db.ProductUnits.AddRange(
            new FridgeChef.Api.Data.Entities.ProductUnit
            {
                ProductId = firstProduct.Id,
                UnitId = unit.Id
            },
            new FridgeChef.Api.Data.Entities.ProductUnit
            {
                ProductId = secondProduct.Id,
                UnitId = unit.Id
            }
        );

        await db.SaveChangesAsync();
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
                unit = "тшт",
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
                unit = "тшт",
                category = "Тестові фрукти",
                isPurchased = true,
                isInFridge = false
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
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
