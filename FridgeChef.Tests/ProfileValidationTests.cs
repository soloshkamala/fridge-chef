using Bunit;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Xunit;

namespace FridgeChef.Tests;

public class ProfileValidationTests
{
    [Fact]
    public async Task EmptyForm_ShouldShowRequiredErrors()
    {
        await using var ctx = new BunitContext();

       
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        
        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization(options =>
            options.ResourcesPath = "Resources");
        ctx.Services.AddScoped<ProfileService>();

        
        var cut = ctx.Render<Profile>();

        cut.WaitForAssertion(() =>
        {
            Assert.NotEmpty(cut.FindAll("form"));
        }, TimeSpan.FromSeconds(5));

        
        cut.Find("#profile-name").Input("");

        
        cut.Find("#profile-email").Input("");

       
        cut.Find("form").Submit();

        
        cut.WaitForAssertion(() =>
        {
            var nameField = cut.Find("#profile-name");
            var emailField = cut.Find("#profile-email");

            Assert.Equal("true", nameField.GetAttribute("aria-invalid"));
            Assert.Equal("true", emailField.GetAttribute("aria-invalid"));
            Assert.Contains("Вкажіть ім'я", cut.Markup);
            Assert.Contains("Вкажіть email", cut.Markup);
        }, TimeSpan.FromSeconds(5));
    }
}