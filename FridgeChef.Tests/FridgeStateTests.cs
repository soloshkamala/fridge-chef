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
    }
}