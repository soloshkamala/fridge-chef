
using Bunit;
using Xunit;
using FridgeChef.Pages;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext
{
    [Fact]
    public void LoginPage_ShowsWelcomeTitle()
    {
        // Перевіряємо заголовок сторінки
        var page = Render<Login>();

        Assert.Contains("Ласкаво просимо", page.Markup);
    }

    [Fact]
    public void LoginPage_ShowsLoginForm()
    {
        // Перевіряємо поля форми входу
        var page = Render<Login>();

        Assert.Contains("Email", page.Markup);
        Assert.Contains("Пароль", page.Markup);
        Assert.Contains("Увійти", page.Markup);
    }
}
