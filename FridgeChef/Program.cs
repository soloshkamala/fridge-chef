
using FridgeChef;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddLocalization();
builder.Services.AddScoped<FridgeChef.Services.ShoppingItemApiService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5039/")
});

var host = builder.Build();

var culture = CultureInfo.CurrentUICulture;

if (culture.Name != "uk-UA" && culture.Name != "en-US")
{
    culture = new CultureInfo("uk-UA");
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();

