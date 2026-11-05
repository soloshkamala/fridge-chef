using FridgeChef.Api.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace FridgeChef.Tests.IntegrationTests;

public class PostgreSqlIntegrationTests
{
    [Fact]
    public async Task PostgreSql_ShouldConnectAndCreateDatabase()
    {
        await using var postgres = new PostgreSqlBuilder()
    .WithImage("postgres:18-alpine")
    .Build();

        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<FridgeChefDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var dbContext = new FridgeChefDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        Assert.True(
            await dbContext.Database.CanConnectAsync());

        Assert.True(
            await dbContext.Users.AnyAsync() == false);
    }
}