using System.Diagnostics;
using FridgeChef.Api.Data;
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace FridgeChef.Tests.Helpers;

public class FridgePlaywrightFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:18-alpine")
            .Build();

    private FridgeKestrelWebApplicationFactory? apiFactory;
    private Process? clientProcess;

    public string ConnectionString =>
        postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<FridgeChefDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var dbContext =
            new FridgeChefDbContext(options);

        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Users.AnyAsync(x => x.Id == 1))
        {
            dbContext.Users.Add(new User
            {
                Id = 1,
                Email = "test@fridgechef.local",
                FirstName = "Test",
                LastName = "User",
                EntraObjectId = "test-object-id",
                EntraTenantId = "test-tenant-id"
            });

            await dbContext.SaveChangesAsync();
        }

        apiFactory = new FridgeKestrelWebApplicationFactory(
            ConnectionString);

        using var client = apiFactory.CreateClient();

        await StartClientAsync();
    }

    private async Task StartClientAsync()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "FridgeChef",
                   "FridgeChef.csproj")))
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new DirectoryNotFoundException(
                "FridgeChef project not found.");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project FridgeChef/FridgeChef.csproj --no-launch-profile --urls http://localhost:5050",
            WorkingDirectory = directory.FullName,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] =
            "Development";

        clientProcess = Process.Start(startInfo);

        if (clientProcess == null)
        {
            throw new InvalidOperationException(
                "Could not start Blazor client.");
        }

        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(3)
        };

        for (int i = 0; i < 120; i++)
        {
            if (clientProcess.HasExited)
            {
                throw new InvalidOperationException(
                    "Blazor client stopped unexpectedly.");
            }

            try
            {
                using var response = await httpClient.GetAsync(
                    "http://localhost:5050/");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        "Blazor client started successfully.");

                    return;
                }
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
            "Blazor client did not start.");
    }

    public async Task DisposeAsync()
    {
        try
        {
            if (clientProcess != null)
            {
                try
                {
                    if (!clientProcess.HasExited)
                    {
                        clientProcess.Kill(
                            entireProcessTree: true);

                        await clientProcess.WaitForExitAsync();
                    }
                }
                finally
                {
                    clientProcess.Dispose();
                }
            }
        }
        finally
        {
            try
            {
                if (apiFactory != null)
                {
                    await apiFactory.DisposeAsync();
                }
            }
            finally
            {
                await postgres.DisposeAsync();
            }
        }
    }
}