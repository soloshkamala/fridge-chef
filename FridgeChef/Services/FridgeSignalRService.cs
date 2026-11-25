using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace FridgeChef.Services;

public class FridgeSignalRService : IAsyncDisposable
{
    private readonly IAccessTokenProvider _tokenProvider;
    private HubConnection? _connection;

    private readonly HashSet<int> _subscribedItemIds = new();

    private readonly string HubUrl;

    private const string ApiScope =
        "api://f9f39e83-370c-4ed6-bfac-f93c39c97542/access_as_user";

    public event Func<Task>? FridgeChanged;

    public FridgeSignalRService(
      IAccessTokenProvider tokenProvider,
      IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;

        var apiAddress = configuration["Api:BaseAddress"]
            ?? throw new InvalidOperationException(
                "Api:BaseAddress is not configured.");

        HubUrl = $"{apiAddress.TrimEnd('/')}/hubs/fridgechef";
    }

    public virtual async Task SubscribeToItemsAsync(IEnumerable<int> itemIds)
    {
        if (_connection?.State != HubConnectionState.Connected)
            return;

        var newIds = itemIds.ToHashSet();

        foreach (var itemId in _subscribedItemIds.Except(newIds).ToArray())
        {
            await _connection.InvokeAsync("LeaveItem", itemId);
        }

        foreach (var itemId in newIds.Except(_subscribedItemIds))
        {
            await _connection.InvokeAsync("JoinItem", itemId);
        }

        _subscribedItemIds.Clear();
        _subscribedItemIds.UnionWith(newIds);
    }

    public virtual async Task StartAsync()
    {
        if (_connection != null)
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl(HubUrl, options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var result = await _tokenProvider.RequestAccessToken(
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
            .WithAutomaticReconnect()
            .Build();

        _connection.On<int>("FridgeItemChanged", async _ =>
        {
            if (FridgeChanged != null)
                await FridgeChanged.Invoke();
        });

        _connection.Reconnecting += error =>
        {
            Console.WriteLine("SignalR: Reconnecting...");
            return Task.CompletedTask;
        };

        _connection.Reconnected += HandleReconnectedAsync;

        await _connection.StartAsync();
        await _connection.InvokeAsync("JoinFridge");
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    private async Task HandleReconnectedAsync(string? connectionId)
    {
        Console.WriteLine("SignalR: Reconnected!");

        await RejoinGroupsAsync();

        Console.WriteLine("SignalR: Groups rejoined!");
    }

    private async Task RejoinGroupsAsync()
    {
        if (_connection?.State != HubConnectionState.Connected)
            return;

        await JoinGroupsAsync(
            (methodName, itemId) =>
                itemId.HasValue
                    ? _connection.InvokeAsync(methodName, itemId.Value)
                    : _connection.InvokeAsync(methodName));
    }

    internal async Task JoinGroupsAsync(
        Func<string, int?, Task> invoke)
    {
        await invoke("JoinFridge", null);

        foreach (var itemId in _subscribedItemIds.ToArray())
        {
            await invoke("JoinItem", itemId);
        }
    }

}