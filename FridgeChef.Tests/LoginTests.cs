
using Bunit;
using Xunit;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using FridgeChef.Pages;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext, IAsyncLifetime
{
    public LoginTests()
    {
        // Підключаємо сервіси MudBlazor
        Services.AddMudServices();

        // Реєструємо TimeProvider
        Services.AddSingleton<TimeProvider>(
            TimeProvider.System
        );

        // Дозволяємо JS-виклики у тестах
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Services.DisposeAsync();
    }

    [Fact]
    public void LoginPage_ShowsWelcomeTitle()
    {
        var page = Render<Login>();

        Assert.Contains("Ласкаво просимо", page.Markup);
    }

    [Fact]
    public void LoginPage_ShowsLoginForm()
    {
        var page = Render<Login>();

        Assert.Contains("Email", page.Markup);
        Assert.Contains("Пароль", page.Markup);
        Assert.Contains("Увійти", page.Markup);
    }
    
[Fact]
public void LoginForm_ShowsErrors_WhenFieldsAreEmpty()
{
    // Створюємо сторінку
    var page = Render<Login>();

    // Натискаємо кнопку входу
    var button = page.Find("button");
    button.Click();

    // Перевіряємо повідомлення про помилки
    Assert.Contains("Введіть електронну пошту", page.Markup);
    Assert.Contains("Введіть пароль", page.Markup);
}

}
