using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;

namespace FridgeChef.Services;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration)
        : base(provider, navigation)
    {
        var apiAddress = configuration["Api:BaseAddress"]
            ?? throw new InvalidOperationException(
                "Api:BaseAddress is not configured.");

        ConfigureHandler(
            authorizedUrls: new[] { apiAddress },
            scopes: new[]
            {
                "api://f9f39e83-370c-4ed6-bfac-f93c39c97542/access_as_user"
            });
    }
}