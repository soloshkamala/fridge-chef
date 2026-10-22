using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Xunit;
using System.Globalization;

namespace FridgeChef.Tests
{
    public abstract class MudTestContext : BunitContext, IAsyncLifetime
    {
        protected MudTestContext()
        {
            var culture = new CultureInfo("uk-UA");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            Services.AddMudServices();
            Services.AddLocalization();

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return DisposeAsync().AsTask();
        }
    }
}