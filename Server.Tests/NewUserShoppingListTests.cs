using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Server.Tests;

public class NewUserShoppingListTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("fridgechef_new_user_test")
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
    public async Task NewMicrosoftUser_CanAccessShoppingList()
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<FridgeChef.Api.Data.FridgeChefDbContext>();

            Assert.False(await db.Users.AnyAsync(x =>
                x.EntraObjectId ==
                    "11111111-1111-1111-1111-111111111111"));
        }

        var response = await client.GetAsync("/api/shopping-items");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();

        var verifyDb = verifyScope.ServiceProvider
            .GetRequiredService<FridgeChef.Api.Data.FridgeChefDbContext>();

        var user = await verifyDb.Users.SingleOrDefaultAsync(x =>
            x.EntraObjectId ==
                "11111111-1111-1111-1111-111111111111" &&
            x.EntraTenantId ==
                "22222222-2222-2222-2222-222222222222");

        Assert.NotNull(user);
        Assert.Equal("shopping-test@example.com", user.Email);
    }
}
