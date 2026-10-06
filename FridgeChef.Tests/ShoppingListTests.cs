using Bunit;
using FridgeChef.Models;
using FridgeChef.Pages;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace FridgeChef.Tests
{
    public class ShoppingListTests
    {
        private BunitContext CreateContext()
        {
            var context = new BunitContext();

            context.JSInterop.Mode = JSRuntimeMode.Loose;
            context.Services.AddMudServices();

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

            Assert.Contains(
                "shopping-loading",
                component.Markup);
        }

        [Fact]
        public async Task ShoppingList_EmptyState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.Empty));

            Assert.Contains(
                "Список покупок порожній",
                component.Markup);

            Assert.Contains(
                "Додай продукти, які потрібно купити.",
                component.Markup);
        }

        [Fact]
        public async Task ShoppingList_ErrorState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.Error));

            Assert.Contains(
                "Не вдалося завантажити список покупок",
                component.Markup);

            Assert.Contains(
                "Спробувати ще раз",
                component.Markup);
        }

        [Fact]
        public async Task ShoppingList_WithDataState()
        {
            await using var context = CreateContext();

            var component = context.Render<ShoppingList>(parameters =>
                parameters.Add(
                    p => p.CurrentState,
                    ShoppingListState.WithData));

            Assert.Contains(
                "Овочі та фрукти",
                component.Markup);

            Assert.Contains(
                "М'ясо та птиця",
                component.Markup);

            Assert.Contains(
                "Помідори",
                component.Markup);

            Assert.Contains(
                "Куряче філе",
                component.Markup);
        }
    }
}