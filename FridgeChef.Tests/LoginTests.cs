
using Bunit;
using Xunit;
using FridgeChef.Pages;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext
{
    [Fact]
    public void LoginPage_ShowsWelcomeTitle()
    {
        // Arrange + Act: створюємо сторінку
        var page = Render<Login>();

        // Assert: перевіряємо заголовок
        Assert.Contains("Ласкаво просимо", page.Markup);
    }
}
