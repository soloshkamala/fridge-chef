using FridgeChef.Api.Features.Products;
using FridgeChef.Api.Services;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using FridgeChef.Api.Hubs;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<FridgeChefDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CurrentUserService>();

// DATA STORES
builder.Services.AddSingleton<CourseStore>();
builder.Services.AddSingleton<ProfileStore>();
builder.Services.AddSingleton<ShoppingItemStore>();
builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<UserPasswordService>();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
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

// COOKIE AUTHENTICATION
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "FridgeChef.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = async context =>
        {
            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized"
            ).ExecuteAsync(context.HttpContext);
        };

        options.Events.OnRedirectToAccessDenied = async context =>
        {
            await Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden"
            ).ExecuteAsync(context.HttpContext);
        };
    });

builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd"),
        jwtBearerScheme: JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiRead", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.FindAll("scp")
                      .Concat(context.User.FindAll(
                          "http://schemas.microsoft.com/identity/claims/scope"))
                      .Any(claim => claim.Value.Split(' ')
                          .Contains("access_as_user"))));

    options.AddPolicy("ApiWrite", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("User", "Admin")
              .RequireAssertion(context =>
                  context.User.FindAll("scp")
                      .Concat(context.User.FindAll(
                          "http://schemas.microsoft.com/identity/claims/scope"))
                      .Any(claim => claim.Value.Split(' ')
                          .Contains("access_as_user"))));

    options.AddPolicy("ApiAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin")
              .RequireAssertion(context =>
                  context.User.FindAll("scp")
                      .Concat(context.User.FindAll(
                          "http://schemas.microsoft.com/identity/claims/scope"))
                      .Any(claim => claim.Value.Split(' ')
                          .Contains("access_as_user"))));
});

// SIGNALR
builder.Services.AddSignalR();

// AZURE MONITOR
var connectionString =
    builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddOpenTelemetry()
        .UseAzureMonitor()
        .WithTracing(tracing =>
        {
            tracing.AddSource("FridgeChef.Api.Fridge");
        });
}

// BUILD
var app = builder.Build();

// MIDDLEWARE
app.UseMiddleware<RequestLoggingMiddleware>();


app.UseCors("Client");

app.UseAuthentication();
app.UseAuthorization();

// RECIPES
app.MapRecipesEndpoints();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
        options.RouteTemplate = "openapi/{documentName}.json");

    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "FridgeChef API v1"));
}

// FRIDGE
app.MapFridgeItemsEndpoints();

// PRODUCTS
app.MapProductsEndpoints();

// COURSES
app.MapCourses();

// SHOPPING ITEMS
app.MapGroup("/api/shopping-items")
    .MapShoppingItemEndpoints()
    .WithTags("Shopping Items");

// PROFILES
app.MapGroup("/api/profiles")
    .MapProfileEndpoints()
    .WithTags("Profiles");

// USERS
app.MapGroup("/api/users")
    .MapUserEndpoints()
    .WithTags("Users");

// AUTHENTICATION
app.MapGroup("/api/auth")
    .MapAuthEndpoints()
    .WithTags("Authentication");

// SIGNALR HUB
app.MapHub<FridgeChefHub>("/hubs/fridgechef");

// RUN
app.Run();

public partial class Program { }
