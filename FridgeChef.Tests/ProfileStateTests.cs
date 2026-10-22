
using System.Globalization;
using Bunit;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Xunit;

namespace FridgeChef.Tests;

public class ProfileStateTests
{
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

            ctx.Services.AddMudServices();
            ctx.Services.AddLocalization(options =>
                options.ResourcesPath = "Resources");

            ctx.Services.AddScoped<ProfileService>();

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

            ctx.JSInterop.Setup<string?>(
                "localStorage.getItem",
                "fridgechef-profile-settings"
            ).SetResult(null);

            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.Services.AddMudServices();
            ctx.Services.AddLocalization(options =>
                options.ResourcesPath = "Resources");

            ctx.Services.AddScoped<ProfileService>(sp =>
                new ProfileService
                {
                    SimulateEmpty = true
                });

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

            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.Services.AddMudServices();
            ctx.Services.AddLocalization(options =>
                options.ResourcesPath = "Resources");

            ctx.Services.AddScoped<ProfileService>(sp =>
                new ProfileService
                {
                    SimulateError = true
                });

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

            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.Services.AddMudServices();
            ctx.Services.AddLocalization(options =>
                options.ResourcesPath = "Resources");

            ctx.Services.AddScoped<ProfileService>();

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
}
