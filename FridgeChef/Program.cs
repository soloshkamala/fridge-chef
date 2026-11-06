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
// API ADDRESS
// =====================================

const string apiAddress = "https://localhost:7173/";

// =====================================
// CATALOGUE API
// =====================================

builder.AddCatalogueApi();

// =====================================
// DEFAULT HTTP CLIENT
// =====================================

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiAddress)
});

// =====================================
// AUTHENTICATION
// =====================================

builder.Services.AddScoped<AuthStateService>();

builder.Services.AddScoped<AuthService>(sp =>
{
    var http = new HttpClient
    {
        BaseAddress = new Uri(apiAddress)
    };

    return new AuthService(http);
});

// =====================================
// APPLICATION SERVICES
// =====================================

builder.Services.AddScoped<FridgeService>();
builder.Services.AddScoped<FridgeItemsApiService>();
builder.Services.AddScoped<RecipesApiService>();
builder.Services.AddScoped<ShoppingItemApiService>();
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