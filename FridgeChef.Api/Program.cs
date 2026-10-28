using FridgeChef.Api.Features.Courses;
using FridgeChef.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CourseStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    var clientOrigin = builder.Configuration["ClientOrigin"]
        ?? throw new InvalidOperationException("The ClientOrigin configuration value is required.");

    options.AddPolicy("Client", policy =>
        policy.WithOrigins(clientOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Client");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "openapi/{documentName}.json");
}

app.MapCourses();

app.Run();

public partial class Program { }
