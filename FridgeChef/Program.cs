
using System.Globalization;
using FridgeChef;
using FridgeChef.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// =====================================
// ROOT COMPONENTS
// =====================================

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// =====================================
// MUDBLAZOR
// =====================================

builder.Services.AddMudServices();

// =====================================
// LOCALIZATION
// =====================================

builder.Services.AddLocalization();

// =====================================
// CATALOGUE API
// =====================================

builder.AddCatalogueApi();

// =====================================
// DEFAULT HTTP CLIENT
// =====================================

// HTTP-клієнт для інших сервісів застосунку
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// =====================================
// AUTHENTICATION
// =====================================

// Стан авторизації
builder.Services.AddScoped<AuthStateService>();

// Окремий HTTP-клієнт для авторизації через API
builder.Services.AddScoped<AuthService>(sp =>
{
    var http = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5039/")
    };

    return new AuthService(http);
});

// =====================================
// FRIDGE SERVICES
// =====================================

builder.Services.AddScoped<FridgeService>();
builder.Services.AddScoped<FridgeItemsApiService>();
builder.Services.AddScoped<ShoppingItemApiService>();

// =====================================
// PROFILE SERVICE
// =====================================

builder.Services.AddScoped<ProfileService>();

// =====================================
// BUILD APPLICATION
// =====================================

var host = builder.Build();

// =====================================
// LOAD SAVED LANGUAGE
// =====================================

var js = host.Services.GetRequiredService<IJSRuntime>();

var savedCulture = await js.InvokeAsync<string>(
    "fridgeCulture.get"
);

if (savedCulture != "uk-UA" && savedCulture != "en-US")
{
    savedCulture = "uk-UA";
}

var culture = new CultureInfo(savedCulture);

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
CultureInfo.CurrentCulture = culture;
CultureInfo.CurrentUICulture = culture;

// =====================================
// RUN APPLICATION
// =====================================

await host.RunAsync();
