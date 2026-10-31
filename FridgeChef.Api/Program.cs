
using FridgeChef.Api.Features.Courses;
using FridgeChef.Api.Features.FridgeItems;
using FridgeChef.Api.Features.ShoppingItems;
using FridgeChef.Api.Features.Profiles;
using FridgeChef.Api.Features.Users;
using FridgeChef.Api.Features.Auth;
using FridgeChef.Api.Middleware;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// DATA STORES
// =====================================

builder.Services.AddSingleton<FridgeItemsStore>();
builder.Services.AddSingleton<CourseStore>();
builder.Services.AddSingleton<ShoppingItemStore>();
builder.Services.AddSingleton<ProfileStore>();

// Користувачі
builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<UserPasswordService>();

// =====================================
// SWAGGER
// =====================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================
// CORS
// =====================================

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
            .AllowAnyMethod()
            .AllowCredentials());
});

// =====================================
// COOKIE AUTHENTICATION
// =====================================

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "FridgeChef.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        // Для API повертаємо HTTP-статус,
        // а не перенаправляємо на сторінку входу.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// =====================================
// BUILD
// =====================================

var app = builder.Build();

// =====================================
// MIDDLEWARE
// =====================================

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("Client");

app.UseAuthentication();
app.UseAuthorization();

// =====================================
// SWAGGER
// =====================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
        options.RouteTemplate = "openapi/{documentName}.json");

    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "FridgeChef API v1"));
}

// =====================================
// FRIDGE
// =====================================

app.MapFridgeItemsEndpoints();

// =====================================
// COURSES
// =====================================

app.MapCourses();

// =====================================
// SHOPPING ITEMS
// =====================================

app.MapGroup("/api/shopping-items")
    .MapShoppingItemEndpoints()
    .WithTags("Shopping Items");

// =====================================
// PROFILES
// =====================================

app.MapGroup("/api/profiles")
    .MapProfileEndpoints()
    .WithTags("Profiles");

// =====================================
// USERS
// =====================================

app.MapGroup("/api/users")
    .MapUserEndpoints()
    .WithTags("Users");

// =====================================
// AUTHENTICATION
// =====================================

app.MapGroup("/api/auth")
    .MapAuthEndpoints()
    .WithTags("Authentication");

// =====================================
// RUN
// =====================================

app.Run();

public partial class Program { }
