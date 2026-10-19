
using Bunit;
using Xunit;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using FridgeChef.Pages;
using FridgeChef.Services;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext, IAsyncLifetime
{
    public LoginTests()
    {
        Services.AddMudServices();

        Services.AddSingleton<TimeProvider>(
            TimeProvider.System
        );

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
        var page = Render<Login>();

        var button = page.Find("button[type='submit']");
        button.Click();

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

    [Fact]
    public void LoginPage_ShowsLoadingState()
    {
        var authState =
            Services.GetRequiredService<AuthStateService>();

        authState.SetLoading();

        var page = Render<Login>();

        Assert.Contains("Завантаження...", page.Markup);
    }
}
