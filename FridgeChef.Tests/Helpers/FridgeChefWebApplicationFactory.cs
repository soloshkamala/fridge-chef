using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace FridgeChef.Tests.Helpers;

public class FridgeChefWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:18-alpine")
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<FridgeChefDbContext>();
            services.RemoveAll<DbContextOptions<FridgeChefDbContext>>();

            services.AddDbContext<FridgeChefDbContext>(options =>
                options.UseNpgsql(postgres.GetConnectionString()));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
        });
    }

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        using var client = CreateClient();

        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Users.AnyAsync(x => x.Id == 1))
        {
            dbContext.Users.Add(new User
            {
                Id = 1,
                Email = "test@fridgechef.local",
                FirstName = "Test",
                LastName = "User"
            });

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task ResetFridgeItemsAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<FridgeChefDbContext>();

        dbContext.FridgeItems.RemoveRange(dbContext.FridgeItems);

        await dbContext.SaveChangesAsync();
    }

    public new async Task DisposeAsync()
    {
        await postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}