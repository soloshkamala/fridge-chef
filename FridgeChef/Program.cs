
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

// Підключення MudBlazor
builder.Services.AddMudServices();

// Підключення локалізації
builder.Services.AddLocalization();

// HTTP-клієнт
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Сервіси авторизації
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthService>();

// Сервіс холодильника
builder.Services.AddScoped<FridgeService>();
builder.Services.AddScoped<FridgeItemsApiService>();

// Сервіс профілю користувача
builder.Services.AddScoped<ProfileService>();

// Створення застосунку
var host = builder.Build();

// Отримання збереженої мови
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

// Запуск застосунку
await host.RunAsync();
