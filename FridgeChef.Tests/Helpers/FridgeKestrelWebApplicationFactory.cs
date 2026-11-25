extern alias FridgeApi;

using FridgeChef.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FridgeChef.Tests.Helpers;

public class FridgeKestrelWebApplicationFactory
    : WebApplicationFactory<FridgeApi::Program>
{
    private readonly string connectionString;
    private IHost? kestrelHost;

    public FridgeKestrelWebApplicationFactory(
        string connectionString)
    {
        this.connectionString = connectionString;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<FridgeChefDbContext>();
            services.RemoveAll<DbContextOptions<FridgeChefDbContext>>();

            services.AddDbContext<FridgeChefDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test", options => { });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();

        builder.ConfigureWebHost(webHost =>
        {
            webHost.UseKestrel();
            webHost.UseUrls("http://localhost:5039");
        });

        kestrelHost = builder.Build();
        kestrelHost.Start();

        testHost.Start();

        return testHost;
    }

    public override async ValueTask DisposeAsync()
    {
        if (kestrelHost != null)
        {
            await kestrelHost.StopAsync();
            kestrelHost.Dispose();
        }

        await base.DisposeAsync();
    }
}