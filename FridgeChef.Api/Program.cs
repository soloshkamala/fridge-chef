
using FridgeChef.Api.Features.Auth;
using FridgeChef.Api.Features.Courses;
using FridgeChef.Api.Features.FridgeItems;
using FridgeChef.Api.Features.Profiles;
using FridgeChef.Api.Features.Recipes;
using FridgeChef.Api.Features.ShoppingItems;
using FridgeChef.Api.Features.Users;
using FridgeChef.Api.Middleware;
using FridgeChef.Api.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// DATABASE
// =====================================

builder.Services.AddDbContext<FridgeChefDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================================
// DATA STORES
// =====================================

builder.Services.AddSingleton<CourseStore>();
builder.Services.AddSingleton<ProfileStore>();
builder.Services.AddSingleton<ShoppingItemStore>();

// Users
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

        // Р”РѕР·РІС–Р» РїРµСЂРµРґР°С‡С– cookie РјС–Р¶ РєР»С–С”РЅС‚РѕРј С‚Р° API
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        // 401 Unauthorized + ProblemDetails
        options.Events.OnRedirectToLogin = async context =>
        {
            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized"
            ).ExecuteAsync(context.HttpContext);
        };

        // 403 Forbidden + ProblemDetails
        options.Events.OnRedirectToAccessDenied = async context =>
        {
            await Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden"
            ).ExecuteAsync(context.HttpContext);
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
// RECIPES
// =====================================

app.MapRecipesEndpoints();

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
// SHOPPING ITEMS - POSTGRESQL
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
