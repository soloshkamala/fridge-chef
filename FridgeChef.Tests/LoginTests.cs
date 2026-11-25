using System;
using System.Globalization;
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
    private readonly CultureInfo previousCulture =
        CultureInfo.CurrentCulture;

    private readonly CultureInfo previousUICulture =
        CultureInfo.CurrentUICulture;

    public LoginTests()
    {
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
        CultureInfo.CurrentUICulture = new CultureInfo("uk-UA");

        Services.AddMudServices();
        Services.AddLocalization();
        Services.AddSingleton<TimeProvider>(TimeProvider.System);

        Services.AddScoped<AuthStateService>();

        JSInterop.Mode = JSRuntimeMode.Loose;

        MockLoginResponse(HttpStatusCode.OK);
    }

    Task IAsyncLifetime.InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();

        CultureInfo.CurrentCulture = previousCulture;
        CultureInfo.CurrentUICulture = previousUICulture;
    }

    [Fact]
    public void LoginPage_ShowsWelcomeTitle()
    {
        var page = Render<Login>();

        Assert.Equal(
            "З поверненням!",
            page.Find("h2").TextContent.Trim()
        );
    }

    [Fact]
    public void LoginPage_ShowsLoginForm()
    {
        var page = Render<Login>();

        Assert.Contains(
            "Електронна пошта",
            page.Markup
        );

        Assert.Contains(
            "Пароль",
            page.Markup
        );

        Assert.NotNull(page.Find("input[type='email']"));
        Assert.NotNull(page.Find("input[type='password']"));

        Assert.Equal(
            "Увійти",
            page.Find("button.btn-primary").TextContent.Trim()
        );

        Assert.Equal(
            "/authentication/login",
            page.Find("a.microsoft-login").GetAttribute("href")
        );
    }

    [Fact]
    public void LoginForm_ShowsErrors_WhenFieldsAreEmpty()
    {
        var page = Render<Login>();

        page.Find("button.btn-primary").Click();

        page.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Заповніть Email та пароль.",
                page.Find(".alert-danger").TextContent
            );
        });
    }

    [Fact]
    public async Task LoginPage_ShowsLoadingState()
    {
        var pendingResponse =
            new TaskCompletionSource<HttpResponseMessage>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

        ConfigureHttpClient(
            new FakeHttpMessageHandler(
                () => pendingResponse.Task
            )
        );

        var page = Render<Login>();

        page.Find("input[type='email']")
            .Change("student@example.com");

        page.Find("input[type='password']")
            .Change("password123");

        var clickTask = page.Find("button.btn-primary")
            .ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        try
        {
            page.WaitForAssertion(() =>
            {
                var button = page.Find("button.btn-primary");

                Assert.True(button.HasAttribute("disabled"));

                Assert.Contains(
                    "Виконується вхід...",
                    button.TextContent
                );
            });
        }
        finally
        {
            pendingResponse.TrySetResult(
                new HttpResponseMessage(HttpStatusCode.Unauthorized)
            );

            await clickTask;
        }
    }

    [Fact]
    public void LoginPage_ShowsErrorState()
    {
        MockLoginResponse(HttpStatusCode.Unauthorized);

        var page = Render<Login>();

        page.Find("input[type='email']")
            .Change("student@example.com");

        page.Find("input[type='password']")
            .Change("wrongpassword");

        page.Find("button.btn-primary").Click();

        page.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Неправильний Email або пароль.",
                page.Find(".alert-danger").TextContent
            );
        });
    }

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

        Assert.True(authState.IsAuthenticated);
        Assert.False(authState.IsLoading);
        Assert.True(string.IsNullOrEmpty(authState.ErrorMessage));
    }

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

    [Fact]
    public void LoginPage_NavigatesToProfileAfterValidSubmit()
    {
        MockLoginResponse(HttpStatusCode.OK);

        var authState =
            Services.GetRequiredService<AuthStateService>();

        var navigation =
            Services.GetRequiredService<NavigationManager>();

        var page = Render<Login>();

        page.Find("input[type='email']")
            .Change("student@example.com");

        page.Find("input[type='password']")
            .Change("password123");

        page.Find("button.btn-primary").Click();

        page.WaitForAssertion(() =>
        {
            Assert.Equal(
                "student@example.com",
                authState.UserEmail
            );

            Assert.True(authState.IsAuthenticated);

            Assert.EndsWith(
                "/profile",
                navigation.Uri
            );
        });
    }

    [Fact]
    public void LoginPage_ShowsErrorAfterInvalidSubmit()
    {
        MockLoginResponse(HttpStatusCode.Unauthorized);

        var authState =
            Services.GetRequiredService<AuthStateService>();

        var navigation =
            Services.GetRequiredService<NavigationManager>();

        var page = Render<Login>();

        page.Find("input[type='email']")
            .Change("student@example.com");

        page.Find("input[type='password']")
            .Change("wrongpassword");

        page.Find("button.btn-primary").Click();

        page.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Неправильний Email або пароль.",
                page.Find(".alert-danger").TextContent
            );

            Assert.False(authState.IsAuthenticated);
            Assert.Null(authState.UserEmail);

            Assert.False(
                navigation.Uri.EndsWith("/profile")
            );

            Assert.False(
                page.Find("button.btn-primary")
                    .HasAttribute("disabled")
            );
        });
    }

    private void MockLoginResponse(HttpStatusCode statusCode)
    {
        ConfigureHttpClient(
            new FakeHttpMessageHandler(
                () => Task.FromResult(
                    new HttpResponseMessage(statusCode)
                )
            )
        );
    }

    private void ConfigureHttpClient(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5039/")
        };

        Services.AddSingleton<HttpClient>(client);

        Services.AddScoped<AuthService>(
            _ => new AuthService(client)
        );
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<Task<HttpResponseMessage>> responseFactory;

        public FakeHttpMessageHandler(
            Func<Task<HttpResponseMessage>> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return responseFactory();
        }
    }
}