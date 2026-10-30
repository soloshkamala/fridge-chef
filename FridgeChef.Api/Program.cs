
using FridgeChef.Api.Features.Courses;
using FridgeChef.Api.Features.ShoppingItems;
using FridgeChef.Api.Features.Profiles;
using FridgeChef.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CourseStore>();
builder.Services.AddSingleton<ShoppingItemStore>();
builder.Services.AddSingleton<ProfileStore>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    var clientOrigin = builder.Configuration["ClientOrigin"]
        ?? throw new InvalidOperationException(
            "The ClientOrigin configuration value is required.");

    options.AddPolicy("Client", policy =>
        policy.WithOrigins(
                clientOrigin,
                "http://localhost:5050",
                "https://localhost:7164")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Client");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
        options.RouteTemplate = "openapi/{documentName}.json");

    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "FridgeChef API v1"));
}

app.MapCourses();

app.MapGroup("/api/shopping-items")
    .MapShoppingItemEndpoints()
    .WithTags("Shopping Items");

app.MapGroup("/api/profiles")
    .MapProfileEndpoints()
    .WithTags("Profiles");

app.Run();

public partial class Program { }
