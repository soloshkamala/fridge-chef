
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace FridgeChef.Services;

public class ShoppingListSignalRService :
    IShoppingListSignalRService,
    IAsyncDisposable
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private HubConnection? _connection;
    private bool _disposed;

    private readonly string HubUrl;

    private const string ApiScope =
        "api://f9f39e83-370c-4ed6-bfac-f93c39c97542/access_as_user";

    public event Func<Task>? ShoppingListChanged;

    public ShoppingListSignalRService(
       IAccessTokenProvider tokenProvider,
       IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;

        var apiAddress = configuration["Api:BaseAddress"]
            ?? throw new InvalidOperationException(
                "Api:BaseAddress is not configured.");

        HubUrl = $"{apiAddress.TrimEnd('/')}/hubs/shopping-list";
    }

    public async Task StartAsync()
    {
        await _connectionLock.WaitAsync();

        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_connection?.State == HubConnectionState.Connected)
                return;

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            var connection = new HubConnectionBuilder()
                .WithUrl(HubUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var result =
                            await _tokenProvider.RequestAccessToken(
                                new AccessTokenRequestOptions
                                {
                                    Scopes = new[] { ApiScope }
                                });

                        if (result.TryGetToken(out var token))
                            return token.Value;

                        throw new InvalidOperationException(
                            "Could not acquire API access token.");
                    };
                })
                .WithAutomaticReconnect(new InfiniteRetryPolicy())
                .Build();

            connection.On<int>(
                "ShoppingItemAdded",
                HandleShoppingItemChanged);

            connection.On<int>(
                "ShoppingItemUpdated",
                HandleShoppingItemChanged);

            connection.On<int>(
                "ShoppingItemDeleted",
                HandleShoppingItemChanged);

            connection.Reconnecting += error =>
            {
                Console.WriteLine(
                    $"Shopping List SignalR reconnecting: {error?.Message}");

                return Task.CompletedTask;
            };

            connection.Reconnected += async connectionId =>
            {
                Console.WriteLine(
                    "Shopping List SignalR reconnected.");

                await JoinShoppingListAsync(connection);
            };

            connection.Closed += error =>
            {
                Console.WriteLine(
                    $"Shopping List SignalR connection closed: {error?.Message}");

                return Task.CompletedTask;
            };

            _connection = connection;

            await connection.StartAsync();

            await JoinShoppingListAsync(connection);

            Console.WriteLine(
                "Shopping List SignalR connected.");
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private static async Task JoinShoppingListAsync(
        HubConnection connection)
    {
        if (connection.State == HubConnectionState.Connected)
        {
            await connection.InvokeAsync("JoinShoppingList");
        }
    }

    private async Task HandleShoppingItemChanged(int itemId)
    {
        Console.WriteLine(
            $"Shopping List item changed: {itemId}");

        var handlers = ShoppingListChanged;

        if (handlers == null)
            return;

        foreach (Func<Task> handler in handlers.GetInvocationList())
        {
            await handler();
        }
    }

    public async Task StopAsync()
    {
        await _connectionLock.WaitAsync();

        try
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await StopAsync();

        _disposed = true;
        _connectionLock.Dispose();
    }
}

public class InfiniteRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(
            System.Math.Min(
                System.Math.Pow(2, retryContext.PreviousRetryCount),
                30));
    }
}
