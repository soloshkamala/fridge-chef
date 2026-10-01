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
    }
}