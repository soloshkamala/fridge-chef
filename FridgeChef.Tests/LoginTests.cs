
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Xunit;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using FridgeChef.Pages;
using FridgeChef.Services;
using FridgeChef.Components;
using FridgeChef.Models;

namespace FridgeChef.Tests;

public class LoginTests : BunitContext, IAsyncLifetime
{
    // Налаштування тестового середовища
    public LoginTests()
    {
        Services.AddMudServices();
        Services.AddSingleton<TimeProvider>(TimeProvider.System);

        Services.AddScoped<AuthStateService>();
        Services.AddScoped<AuthService>();

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

    // Тест 2. Перевірка наявності форми
    [Fact]
    public void LoginPage_ShowsLoginForm()
    {
        var page = Render<Login>();

        Assert.Contains("Email", page.Markup);
        Assert.Contains("Пароль", page.Markup);

        Assert.Contains(
            "Увійти",
            page.Markup,
            StringComparison.OrdinalIgnoreCase
        );
    }

    // Тест 3. Валідація порожніх полів
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

        Assert.Contains(
            "Виконується вхід...",
            page.Markup
        );
    }

    // Тест 5. Відображення повідомлення про помилку
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

    // Тест 6. Успішний стан авторизації
    [Fact]
    public void LoginPage_SetsSuccessState()
    {
        var authState =
            Services.GetRequiredService<AuthStateService>();

        authState.SetSuccess("student@example.com");

        Assert.Equal(
            "student@example.com",
            authState.UserEmail
        );

        Assert.False(authState.IsLoading);
        Assert.True(string.IsNullOrEmpty(authState.ErrorMessage));
    }

    // Тест 7. Перевірка виклику EventCallback
    [Fact]
    public void LoginForm_CallsCallback_WhenSubmitted()
    {
        bool callbackCalled = false;

        var page = Render<LoginForm>(
            parameters => parameters.Add(
                p => p.OnLogin,
                (LoginModel model) =>
                {
                    callbackCalled = true;
                }
            )
        );

        page.FindAll("input")[0]
            .Change("student@example.com");

        page.FindAll("input")[1]
            .Change("password123");

        page.Find("form").Submit();

        Assert.True(callbackCalled);
    }

    // Тест 8. Передавання Email і пароля
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

        Assert.Equal(
            "student@example.com",
            receivedModel.Email
        );

        Assert.Equal(
            "password123",
            receivedModel.Password
        );
    }

    // Тест 9. Успішний вхід і перехід на Profile
    [Fact]
    public void LoginPage_NavigatesToProfileAfterValidSubmit()
    {
        // Імітуємо успішну відповідь сервера
        MockLoginResponse(HttpStatusCode.OK);

        var authState =
            Services.GetRequiredService<AuthStateService>();

        var navigation =
            Services.GetRequiredService<NavigationManager>();

        var page = Render<Login>();

        page.FindAll("input")[0]
            .Change("student@example.com");

        page.FindAll("input")[1]
            .Change("password123");

        page.Find("form").Submit();

        page.WaitForAssertion(() =>
        {
            Assert.Equal(
                "student@example.com",
                authState.UserEmail
            );

            Assert.EndsWith(
                "/profile",
                navigation.Uri
            );
        });
    }

    // Тест 10. Помилка входу з неправильним паролем
    [Fact]
    public void LoginPage_ShowsErrorAfterInvalidSubmit()
    {
        // Імітуємо відмову сервера
        MockLoginResponse(HttpStatusCode.Unauthorized);

        var page = Render<Login>();

        page.FindAll("input")[0]
            .Change("student@example.com");

        page.FindAll("input")[1]
            .Change("wrongpassword");

        page.Find("form").Submit();

        page.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Неправильний Email або пароль",
                page.Markup
            );
        });
    }

    // Допоміжний метод для імітації відповіді API
    private void MockLoginResponse(HttpStatusCode statusCode)
    {
        var client = new HttpClient(
            new FakeHttpMessageHandler(statusCode))
        {
            BaseAddress = new Uri("http://localhost:5039/")
        };

        Services.AddScoped<AuthService>(
            _ => new AuthService(client)
        );
    }

    // Імітація HTTP-сервера для тестів
    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new HttpResponseMessage(_statusCode)
            );
        }
    }
}
