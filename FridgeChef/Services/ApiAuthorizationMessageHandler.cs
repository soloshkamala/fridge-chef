using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FridgeChef.Services;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation)
        : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[] { "http://localhost:5039/" },
            scopes: new[]
            {
                "api://f9f39e83-370c-4ed6-bfac-f93c39c97542/access_as_user"
            });
    }
}
