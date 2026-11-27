
using Bunit;
using FridgeChef.Models;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Globalization;
using System.Net.Http;

namespace FridgeChef.Tests
{
    public class ShoppingListTests
    {
        private BunitContext CreateContext()
        {
            var context = new BunitContext();

            var culture = new CultureInfo("uk-UA");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            context.JSInterop.Mode = JSRuntimeMode.Loose;

            context.Services.AddMudServices();
            context.Services.AddLocalization();

            context.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5039/")
            });

            context.Services.AddScoped<ShoppingItemApiService>();

            context.Services.AddSingleton<
                IShoppingListSignalRService,
                FakeShoppingListSignalRService>();

            return context;
        }

        [Fact]
        public async Task ShoppingList_LoadingState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.Loading));

            Assert.Contains("shopping-loading", component.Markup);
        }

        [Fact]
        public async Task ShoppingList_EmptyState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.Empty));

            Assert.Contains("shopping-state-card", component.Markup);
        }

        [Fact]
        public async Task ShoppingList_ErrorState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.Error));

            Assert.Contains("error-state", component.Markup);
        }

        [Fact]
        public async Task ShoppingList_WithDataState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.WithData));

            Assert.DoesNotContain("shopping-loading", component.Markup);
        }
    }

    public class FakeShoppingListSignalRService :
        IShoppingListSignalRService
    {
        private Func<Task>? _shoppingListChanged;

        public event Func<Task>? ShoppingListChanged
        {
            add => _shoppingListChanged += value;
            remove => _shoppingListChanged -= value;
        }

        public Task NotifyShoppingListChangedAsync()
        {
            return _shoppingListChanged?.Invoke() ?? Task.CompletedTask;
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
