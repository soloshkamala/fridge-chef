
using System.Globalization;
using FridgeChef;
using FridgeChef.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddLocalization();

builder.AddCatalogueApi();

builder.Services.AddScoped<ShoppingItemApiService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FridgeService>();
builder.Services.AddScoped<ProfileService>();

var host = builder.Build();

var js = host.Services.GetRequiredService<IJSRuntime>();
var savedCulture = await js.InvokeAsync<string>("fridgeCulture.get");

if (savedCulture != "uk-UA" && savedCulture != "en-US")
{
    savedCulture = "uk-UA";
}

var culture = new CultureInfo(savedCulture);

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
CultureInfo.CurrentCulture = culture;
CultureInfo.CurrentUICulture = culture;

await host.RunAsync();
