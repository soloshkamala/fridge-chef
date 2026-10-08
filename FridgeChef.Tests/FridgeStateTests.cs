using Bunit;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FridgeChef.Tests
{
    public class FridgeStateTests : MudTestContext
    {
        [Fact]
        public void Empty_fridge_shows_empty_state()
        {
            Services.AddScoped<FridgeService>();

            var page = Render<Fridge>();

            Assert.Contains("Ваш холодильник порожній", page.Markup);
        }

        [Fact]
        public void Loading_fridge_shows_loading_state()
        {
            Services.AddScoped<FridgeService>();

            var service = Services.GetRequiredService<FridgeService>();
            service.IsLoading = true;

            var page = Render<Fridge>();

            Assert.Contains("Завантаження...", page.Markup);
        }

        [Fact]
        public void Error_fridge_shows_error_state()
        {
            Services.AddScoped<FridgeService>();

            var service = Services.GetRequiredService<FridgeService>();
            service.ErrorMessage = "Не вдалося завантажити продукти";

            var page = Render<Fridge>();

            Assert.Contains("Не вдалося завантажити продукти", page.Markup);
        }

        [Fact]
        public void Data_fridge_shows_products_and_recipes_section()
        {
            Services.AddScoped<FridgeService>();

            var service = Services.GetRequiredService<FridgeService>();
            var ingredient = service.Ingredients.First(x => x.Name == "Молоко");

            service.AddToFridge(ingredient, 1, "л");

            var page = Render<Fridge>();

            Assert.Contains("Молоко", page.Markup);
            Assert.Contains("Підібрані рецепти", page.Markup);
        }

        [Fact]
        public void Search_shows_message_when_no_products_found()
        {
            Services.AddScoped<FridgeService>();

            var page = Render<Fridge>();

            var search = page.Find("input");
            search.Input("Ананас");

            Assert.Contains("Продуктів не знайдено", page.Markup);
        }

        [Fact]
        public void AddToFridge_rejects_negative_quantity()
        {
            var service = new FridgeService();

            var milk = service.Ingredients
                .First(x => x.Name == "Молоко");

            service.AddToFridge(milk, -5, "мл");

            Assert.Empty(service.FridgeItems);
        }
    }
}