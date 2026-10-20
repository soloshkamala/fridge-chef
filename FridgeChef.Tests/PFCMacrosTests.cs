using Bunit;
using FridgeChef.Pages;
using System.Globalization;
using Xunit;

namespace FridgeChef.Tests
{
    public class PFCMacrosTests : MudTestContext
    {
        [Fact]
        public void PFCMacrosPage_ShouldRenderTitleAndElements()
        {
            // Встановлюємо українську локалізацію
            var culture = new CultureInfo("uk-UA");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            // Arrange & Act — рендеримо сторінку
            var cut = Render<PFCMacros>();

            // Assert — перевіряємо наявність заголовок та тексту на сторінці
            var markup = cut.Markup;
            Assert.Contains("Калькулятор КБЖВ", markup);
        }
    }
}