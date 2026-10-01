using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Xunit;

namespace FridgeChef.Tests
{
    public abstract class MudTestContext : BunitContext, IAsyncLifetime
    {
        protected MudTestContext()
        {
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