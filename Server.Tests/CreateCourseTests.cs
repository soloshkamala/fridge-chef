using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Server.Tests;

public sealed class CreateCourseTests
{
    [Fact]
    public async Task CreateCourse_DuplicateCodeReturnsConflictProblem()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var request = new
        {
            Code = "CS101",
            Title = "Introduction to Computing",
            Credits = 3,
            TeacherId = 1
        };

        using var created = await client.PostAsJsonAsync("/api/courses", request);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        using var conflict = await client.PostAsJsonAsync("/api/courses", request);

        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        Assert.StartsWith(
            "application/problem+json",
            conflict.Content.Headers.ContentType?.ToString(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateCourse_InvalidInputReturnsValidationProblemDetails()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/courses", new
        {
            Code = "CS101",
            Title = "",
            Credits = 0,
            TeacherId = 1
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.StartsWith(
            "application/problem+json",
            response.Content.Headers.ContentType?.ToString(),
            StringComparison.OrdinalIgnoreCase);

        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = problem.RootElement.GetProperty("errors");

        Assert.True(errors.TryGetProperty("Credits", out _));
        Assert.True(errors.TryGetProperty("Title", out _));
    }
}
