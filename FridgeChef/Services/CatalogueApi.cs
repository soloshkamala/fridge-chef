using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FridgeChef.Services;

public static class CatalogueApi
{
    public static WebAssemblyHostBuilder AddCatalogueApi(
        this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<ICourseSource, ApiCourseSource>();

        return builder;
    }
}
