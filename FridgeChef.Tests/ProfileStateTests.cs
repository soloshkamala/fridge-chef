using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Bunit;
using Bunit.TestDoubles;
using FridgeChef.Models;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Xunit;

namespace FridgeChef.Tests;

public class ProfileStateTests
{
    private static void ConfigureContext(
        BunitContext ctx,
        bool simulateEmpty = false,
        bool simulateError = false)
    {
        var authContext = ctx.AddAuthorization();

        authContext.SetAuthorized("Name Surname");
        authContext.SetRoles("User");

        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        ctx.JSInterop.Setup<string?>(
            "localStorage.getItem",
            "fridgechef-profile-settings"
        ).SetResult(null);

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization();

        ctx.Services.AddScoped<ProfileService>(_ =>
            new ProfileService
            {
                SimulateEmpty = simulateEmpty,
                SimulateError = simulateError
            });

        ctx.Services.AddSingleton(new HttpClient(
            new ProfileHttpMessageHandler(simulateEmpty))
        {
            BaseAddress = new Uri("http://localhost:5039/")
        });
    }

    [Fact]
    public async Task Profile_ShouldShowLoadingState()
    {
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            await using var ctx = new BunitContext();

            ConfigureContext(ctx);

            var cut = ctx.Render<Profile>();

            Assert.NotEmpty(
                cut.FindAll("[data-testid='loading']")
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
            CultureInfo.CurrentUICulture = oldUICulture;
        }
    }

    [Fact]
    public async Task Profile_ShouldShowEmptyState()
    {
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            await using var ctx = new BunitContext();

            ConfigureContext(ctx, simulateEmpty: true);

            var cut = ctx.Render<Profile>();

            cut.WaitForAssertion(() =>
            {
                Assert.NotEmpty(
                    cut.FindAll("[data-testid='empty']")
                );
            }, TimeSpan.FromSeconds(5));
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
            CultureInfo.CurrentUICulture = oldUICulture;
        }
    }

    [Fact]
    public async Task Profile_ShouldShowErrorState()
    {
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            await using var ctx = new BunitContext();

            ConfigureContext(ctx, simulateError: true);

            var cut = ctx.Render<Profile>();

            cut.WaitForAssertion(() =>
            {
                Assert.NotEmpty(
                    cut.FindAll("[data-testid='error']")
                );

                var alert = cut.Find(".mud-alert");

                Assert.Contains(
                    "Failed to load profile data and saved recipes.",
                    alert.TextContent
                );
            }, TimeSpan.FromSeconds(5));
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
            CultureInfo.CurrentUICulture = oldUICulture;
        }
    }

    [Fact]
    public async Task Profile_ShouldShowDataState()
    {
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            await using var ctx = new BunitContext();

            ConfigureContext(ctx);

            var cut = ctx.Render<Profile>();

            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Name Surname", cut.Markup);
                Assert.Contains("Chicken Pasta", cut.Markup);
                Assert.Contains("Avocado Toast", cut.Markup);
            }, TimeSpan.FromSeconds(5));

            Assert.Empty(
                cut.FindAll("[data-testid='loading']")
            );

            Assert.Empty(
                cut.FindAll("[data-testid='error']")
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
            CultureInfo.CurrentUICulture = oldUICulture;
        }
    }

    private sealed class ProfileHttpMessageHandler : HttpMessageHandler
    {
        private readonly bool simulateEmpty;

        public ProfileHttpMessageHandler(bool simulateEmpty)
        {
            this.simulateEmpty = simulateEmpty;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath;

            if (path == "/api/auth/me")
            {
                return Task.FromResult(new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new
                    {
                        Name = "Name Surname",
                        Email = "test@fridgechef.local",
                        Roles = new[] { "User" }
                    })
                });
            }

            if (path == "/api/recipes/saved")
            {
                var recipes = new List<RecipeSummary>();

                if (!simulateEmpty)
                {
                    recipes.Add(new RecipeSummary(
                        "1",
                        "Chicken Pasta",
                        "https://example.com/pasta.jpg",
                        25,
                        440
                    ));

                    recipes.Add(new RecipeSummary(
                        "2",
                        "Avocado Toast",
                        "https://example.com/toast.jpg",
                        15,
                        320
                    ));
                }

                return Task.FromResult(new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(recipes)
                });
            }

            return Task.FromResult(new HttpResponseMessage(
                HttpStatusCode.NotFound));
        }
    }
}