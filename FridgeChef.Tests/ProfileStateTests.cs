
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
        await using var ctx = new BunitContext();

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        ctx.Services.AddScoped<ProfileService>();

        var cut = ctx.Render<Profile>();

        Assert.NotEmpty(
            cut.FindAll("[data-testid='loading']")
        );
    }

    [Fact]
    public async Task Profile_ShouldShowEmptyState()
    {
        await using var ctx = new BunitContext();

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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

    [Fact]
    public async Task Profile_ShouldShowErrorState()
    {
        await using var ctx = new BunitContext();

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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
        }, TimeSpan.FromSeconds(5));

        Assert.Contains(
            "Не вдалося завантажити",
            cut.Markup);
    }

    [Fact]
    public async Task Profile_ShouldShowDataState()
    {
        await using var ctx = new BunitContext();

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        ctx.Services.AddScoped<ProfileService>();

        var cut = ctx.Render<Profile>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Name Surname", cut.Markup);
            Assert.Contains("Паста з куркою", cut.Markup);
            Assert.Contains("Авокадо тост", cut.Markup);
        }, TimeSpan.FromSeconds(5));

        Assert.Empty(
            cut.FindAll("[data-testid='loading']")
        );

        Assert.Empty(
            cut.FindAll("[data-testid='error']")
        );
    }


}
