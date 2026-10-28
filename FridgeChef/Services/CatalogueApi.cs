using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FridgeChef.Services;

public static class CatalogueApi
{
    public static WebAssemblyHostBuilder AddCatalogueApi(this WebAssemblyHostBuilder builder)
    {
        var baseAddress = builder.Configuration["Api:BaseAddress"] ?? "https://localhost:7101/";

        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri(baseAddress, UriKind.Absolute)
        });
        builder.Services.AddScoped<ICourseSource, ApiCourseSource>();

        return builder;
    }
}
