using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Bunit;
using Bunit.TestDoubles;
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
        var oldCulture = CultureInfo.CurrentCulture;
        var oldUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
            CultureInfo.CurrentUICulture = new CultureInfo("uk-UA");

            await using var ctx = new BunitContext();

            var authContext = ctx.AddAuthorization();
            authContext.SetAuthorized("Test User");
            authContext.SetRoles("User");

            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.JSInterop.Setup<string?>(
                "localStorage.getItem",
                "fridgechef-profile-settings"
            ).SetResult(null);

            ctx.Services.AddMudServices();
            ctx.Services.AddLocalization(options =>
                options.ResourcesPath = "Resources");

            ctx.Services.AddSingleton(new HttpClient(
                new ProfileValidationHttpHandler())
            {
                BaseAddress = new Uri("http://localhost:5039/")
            });

            ctx.Services.AddScoped<ProfileApiService>();
            ctx.Services.AddScoped<ProfileService>();

            var cut = ctx.Render<Profile>();

            cut.WaitForAssertion(() =>
            {
                Assert.NotEmpty(cut.FindAll("form"));
            }, TimeSpan.FromSeconds(10));

            cut.Find("#profile-name").Input("");
            cut.Find("#profile-email").Input("");

            cut.Find("form").Submit();

            cut.WaitForAssertion(() =>
            {
                var nameField = cut.Find("#profile-name");
                var emailField = cut.Find("#profile-email");

                Assert.Equal(
                    "true",
                    nameField.GetAttribute("aria-invalid"));

                Assert.Equal(
                    "true",
                    emailField.GetAttribute("aria-invalid"));

                Assert.Contains("Вкажіть ім'я", cut.Markup);
                Assert.Contains("Вкажіть email", cut.Markup);

            }, TimeSpan.FromSeconds(5));
        }
        finally
        {
            CultureInfo.CurrentCulture = oldCulture;
            CultureInfo.CurrentUICulture = oldUICulture;
        }
    }

    private sealed class ProfileValidationHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath;

            if (path == "/api/profiles/me")
            {
                return Task.FromResult(new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create<object?>(null)
                });
            }

            if (path == "/api/auth/me")
            {
                return Task.FromResult(new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new
                    {
                        Name = "Test User",
                        Email = "test@fridgechef.local",
                        Roles = new[] { "User" }
                    })
                });
            }

            if (path == "/api/recipes/saved")
            {
                return Task.FromResult(new HttpResponseMessage(
                    HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        Array.Empty<object>())
                });
            }

            return Task.FromResult(new HttpResponseMessage(
                HttpStatusCode.NotFound));
        }
    }
}