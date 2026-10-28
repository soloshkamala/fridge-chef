
using FridgeChef.Api.Features.ShoppingItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ShoppingItemStore>();

// Дозволяємо запити від клієнта.
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5050",
                "https://localhost:7164")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Логування HTTP-запитів.
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
            "{Method} {Path} responded {StatusCode} in {Elapsed} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
});

// Swagger тільки в режимі Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS перед кінцевими точками API.
app.UseCors("ClientPolicy");

// API списку покупок.
app.MapGroup("/api/shopping-items")
    .MapShoppingItemEndpoints()
    .WithTags("Shopping Items");

app.Run();

public partial class Program { }
