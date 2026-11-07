
using System.Net;
using System.Net.Http.Json;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using FridgeChef.Api.Features.Profiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace FridgeChef.Tests.IntegrationTests;

public class ProfileApiIntegrationTests
{
    [Fact]
    public async Task CreateProfile_ShouldReturn201_AndGetByLocation_ShouldReturn200()
    {
        
        await using var postgres = new PostgreSqlBuilder()
            .WithImage("postgres:18-alpine")
            .Build();

        await postgres.StartAsync();

       
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(
                    "ConnectionStrings:DefaultConnection",
                    postgres.GetConnectionString());

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ClientOrigin"] = "http://localhost:5050"
                        });
                });
            });

        
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<FridgeChefDbContext>();

            await db.Database.MigrateAsync();

           
            db.Users.Add(new User
            {
                Email = "integration.profile@example.com",
                FirstName = "Integration",
                LastName = "User"
            });

            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });

       
        var request = new CreateProfileRequest(
            "Integration User",
            "integration.profile@example.com",
            new List<string> { "Vegetarian", "Gluten Free" },
            "Tomato, Rice"
        );

        var postResponse = await client.PostAsJsonAsync(
            "/api/profiles/", request);

        
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        var location = postResponse.Headers.Location!;

        
        var getResponse = await client.GetAsync(location);

        
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var profile = await getResponse.Content
            .ReadFromJsonAsync<ProfileDto>();

        Assert.NotNull(profile);
        Assert.Equal("integration.profile@example.com", profile.Email);
        Assert.Contains("Vegetarian", profile.Diets);
        Assert.Contains("Gluten Free", profile.Diets);
        Assert.Equal("Tomato, Rice", profile.FavoriteIngredients);
    }
}
