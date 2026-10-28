using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace FridgeChef.Services;

public static class ProfileCulture
{
    public static async Task ApplyAsync(WebAssemblyHost host)
    {
        var js = host.Services.GetRequiredService<IJSRuntime>();

        var stored = await js.InvokeAsync<string?>(
            "localStorage.getItem",
            "profile-language");

        var cultureName = stored is "uk-UA" or "en-US"
            ? stored
            : "uk-UA";

        var culture = CultureInfo.GetCultureInfo(cultureName);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}