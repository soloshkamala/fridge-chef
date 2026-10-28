using FridgeChef.Api.Features.FridgeItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FridgeItemsStore>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientOnly", policy =>
    {
        policy.WithOrigins("http://localhost:5050")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FridgeChef API v1");
    });
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    try
    {
        await next();
    }
    finally
    {
        stopwatch.Stop();

        app.Logger.LogInformation(
            "{Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
});
app.UseCors("ClientOnly");

app.MapFridgeItemsEndpoints();

app.Run();

public partial class Program { }