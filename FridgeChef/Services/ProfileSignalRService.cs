
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace FridgeChef.Services;

public class ProfileSignalRService : IAsyncDisposable
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly string _hubUrl;

    private HubConnection? _connection;

    private const string ApiScope =
        "api://f9f39e83-370c-4ed6-bfac-f93c39c97542/access_as_user";

    public event Func<Task>? ProfileChanged;

    public ProfileSignalRService(
        IAccessTokenProvider tokenProvider,
        IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;

        var apiAddress = configuration["Api:BaseAddress"]
            ?? throw new InvalidOperationException(
                "Api:BaseAddress is not configured.");

        _hubUrl = $"{apiAddress.TrimEnd('/')}/hubs/profile";
    }

    public async Task StartAsync()
    {
        if (_connection != null)
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
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

        _connection.On("ProfileChanged", async () =>
        {
            if (ProfileChanged != null)
                await ProfileChanged.Invoke();
        });

        _connection.Reconnected += async _ =>
        {
            await JoinProfileAsync();
        };

        await _connection.StartAsync();
        await JoinProfileAsync();
    }

    private async Task JoinProfileAsync()
    {
        if (_connection?.State != HubConnectionState.Connected)
            return;

        await _connection.InvokeAsync("JoinProfile");
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
