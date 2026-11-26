
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.Http.Json;
using System.Text.Json;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Server.Tests;

public class ShoppingListE2ETests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("shopping_list_e2e")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private Process? apiProcess;
    private HttpClient client = null!;
    private string apiUrl = "";

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<FridgeChefDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using (var db = new FridgeChefDbContext(options))
        {
            await db.Database.MigrateAsync();

            // USER
            db.Users.Add(new User
            {
                Email = "shopping-test@example.com",
                FirstName = "Test",
                LastName = "User",
                EntraObjectId = "11111111-1111-1111-1111-111111111111",
                EntraTenantId = "22222222-2222-2222-2222-222222222222"
            });

            await db.SaveChangesAsync();

            // CATEGORY
            var category = new Category
            {
                Name = "Тестові фрукти"
            };

            // UNIT
            var unit = new FridgeChef.Api.Data.Entities.Unit
            {
                Name = "Тестові штуки",
                ShortName = "тшт"
            };

            db.Categories.Add(category);
            db.Units.Add(unit);

            await db.SaveChangesAsync();

            // PRODUCTS
            var firstProduct = new Product
            {
                Name = "Тестові яблука",
                CategoryId = category.Id
            };

            var secondProduct = new Product
            {
                Name = "Зелені яблука",
                CategoryId = category.Id
            };

            db.Products.AddRange(firstProduct, secondProduct);

            await db.SaveChangesAsync();

            // PRODUCT UNITS
            db.ProductUnits.AddRange(
                new ProductUnit
                {
                    ProductId = firstProduct.Id,
                    UnitId = unit.Id
                },
                new ProductUnit
                {
                    ProductId = secondProduct.Id,
                    UnitId = unit.Id
                });

            await db.SaveChangesAsync();
        }

        // FIND FREE PORT
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        apiUrl = $"http://127.0.0.1:{port}";

        // START KESTREL
        var apiDirectory = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../FridgeChef.Api"));

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = apiDirectory,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--no-build");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add(apiUrl);

        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "E2E";

        startInfo.Environment["ConnectionStrings__DefaultConnection"] =
            postgres.GetConnectionString();

        startInfo.Environment["ClientOrigin"] =
            "http://localhost:5050";

        apiProcess = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Could not start the API process.");

        client = new HttpClient
        {
            BaseAddress = new Uri(apiUrl),
            Timeout = TimeSpan.FromSeconds(5)
        };

        client.DefaultRequestHeaders.Add(
            "X-Test-User",
            "user1");

        await WaitForApiAsync();
    }

    public async Task DisposeAsync()
    {
        client?.Dispose();

        if (apiProcess is not null)
        {
            if (!apiProcess.HasExited)
            {
                apiProcess.Kill(entireProcessTree: true);
                await apiProcess.WaitForExitAsync();
            }

            apiProcess.Dispose();
        }

        await postgres.DisposeAsync();
    }

    [Fact]
    public async Task ShoppingList_CreateGetUpdateDelete_WorksThroughKestrel()
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

        // GET ONE ITEM
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

        var id = item.GetProperty("id").GetInt32();

        // GET ALL ITEMS
        var listResponse = await client.GetAsync(
            "/api/shopping-items");

        Assert.Equal(
            HttpStatusCode.OK,
            listResponse.StatusCode);

        var items =
            await listResponse.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        Assert.True(items.GetArrayLength() >= 1);

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

        // VERIFY UPDATE
        var updatedResponse = await client.GetAsync(
            $"/api/shopping-items/{id}");

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
        var deleteResponse = await client.DeleteAsync(
            $"/api/shopping-items/{id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // VERIFY DELETE
        var afterDeleteResponse = await client.GetAsync(
            $"/api/shopping-items/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            afterDeleteResponse.StatusCode);
    }

    private async Task WaitForApiAsync()
    {
        using var readinessClient = new HttpClient
        {
            BaseAddress = new Uri(apiUrl),
            Timeout = TimeSpan.FromSeconds(2)
        };

        readinessClient.DefaultRequestHeaders.Add(
            "X-Test-User",
            "user1");

        for (var attempt = 0; attempt < 30; attempt++)
        {
            if (apiProcess!.HasExited)
            {
                throw new InvalidOperationException(
                    $"API process exited with code {apiProcess.ExitCode}.");
            }

            try
            {
                var response = await readinessClient.GetAsync(
                    "/api/shopping-items/");

                if (response.StatusCode == HttpStatusCode.OK)
                    return;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException(
            "The API did not become ready within 30 seconds.");
    }
}
