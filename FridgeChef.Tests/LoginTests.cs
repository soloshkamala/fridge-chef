
using Bunit;
using Xunit;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using FridgeChef.Pages;
using FridgeChef.Services;
using FridgeChef.Components;
using FridgeChef.Components;
using FridgeChef.Models;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext, IAsyncLifetime
{
    public LoginTests()
    {
        Services.AddMudServices();
        Services.AddSingleton<TimeProvider>(TimeProvider.System);
        Services.AddScoped<AuthStateService>();

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    Task IAsyncLifetime.InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
    }

    // Тест 1. Перевірка заголовка сторінки
    [Fact]
    public void LoginPage_ShowsWelcomeTitle()
    {
        var page = Render<Login>();

        Assert.Contains("Ласкаво просимо", page.Markup);
    }

    // Тест 2. Перевірка форми авторизації
    [Fact]
    public void LoginPage_ShowsLoginForm()
    {
        var page = Render<Login>();

        Assert.Contains("Email", page.Markup);
        Assert.Contains("Пароль", page.Markup);
        Assert.Contains("Увійти", page.Markup);
    }

    // Тест 3. Перевірка валідації порожніх полів
    [Fact]
    public void LoginForm_ShowsErrors_WhenFieldsAreEmpty()
    {
        var page = Render<Login>();

        page.Find("button[type='submit']").Click();

        page.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Введіть електронну пошту",
                page.Markup
            );

            Assert.Contains(
                "Введіть пароль",
                page.Markup
            );
        });
    }

    // Тест 4. Перевірка стану завантаження
    [Fact]
    public void LoginPage_ShowsLoadingState()
    {
        var authState =
            Services.GetRequiredService<AuthStateService>();

        authState.SetLoading();

        var page = Render<Login>();

        Assert.Contains("Завантаження...", page.Markup);
    }

    // Тест 5. Перевірка повідомлення про помилку
    [Fact]
    public void LoginPage_ShowsErrorState()
    {
        var authState =
            Services.GetRequiredService<AuthStateService>();

        authState.SetError("Неправильний Email або пароль");

        var page = Render<Login>();

        Assert.Contains(
            "Неправильний Email або пароль",
            page.Markup
        );
    }

    // Тест 6. Перевірка успішного входу
    [Fact]
    public void LoginPage_ShowsSuccessState()
    {
        var authState =
            Services.GetRequiredService<AuthStateService>();

        authState.SetSuccess("student@example.com");

        var page = Render<Login>();

        Assert.Contains(
            "Вхід виконано успішно",
            page.Markup
        );

        Assert.Contains(
            "student@example.com",
            page.Markup
        );
    }

    // Тест 7. Перевірка виклику EventCallback
    [Fact]
    public void LoginForm_CallsCallback_WhenSubmitted()
    {
        bool callbackCalled = false;

        var page = Render<LoginForm>(
            parameters => parameters.Add(
                p => p.OnLogin,
                (LoginModel model) => { callbackCalled = true; }
            )
        );

        // Вводимо Email
        page.FindAll("input")[0]
            .Change("student@example.com");

        // Повторно знаходимо поля після оновлення
        page.FindAll("input")[1]
            .Change("password123");

        // Надсилаємо форму
        page.Find("form").Submit();

        // Перевіряємо, чи викликався EventCallback
        Assert.True(callbackCalled);
    }
    
[Fact]
public void LoginPage_ShowsLoadingAfterValidSubmit()
{
    var page = Render<Login>();

    page.FindAll("input")[0]
        .Change("student@example.com");

    page.FindAll("input")[1]
        .Change("password123");

    page.Find("form").Submit();

    page.WaitForAssertion(() =>
    {
        Assert.Contains("Завантаження...", page.Markup);
    });
}


[Fact]
public void LoginForm_PassesEmailAndPassword()
{
    LoginModel? receivedModel = null;

    var page = Render<LoginForm>(
        parameters => parameters.Add(
            p => p.OnLogin,
            (LoginModel model) =>
            {
                receivedModel = model;
            }
        )
    );

    page.FindAll("input")[0]
        .Change("student@example.com");

    page.FindAll("input")[1]
        .Change("password123");

    page.Find("form").Submit();

    Assert.NotNull(receivedModel);
    Assert.Equal("student@example.com", receivedModel.Email);
    Assert.Equal("password123", receivedModel.Password);
}

}
