using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Server.Tests;

public sealed class CourseLifecycleTests
{
    [Fact]
    public async Task Course_CanBeCreatedListedReadReplacedAndDeleted()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var course = new
        {
            Code = "CS102",
            Title = "Data Structures",
            Credits = 4,
            TeacherId = 2
        };

        using var created = await client.PostAsJsonAsync("/api/courses", course);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var location = Assert.IsType<Uri>(created.Headers.Location);

        using var list = await client.GetAsync("/api/courses");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);

        using var read = await client.GetAsync(location);
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);

        using var replaced = await client.PutAsJsonAsync(location, new
        {
            Code = course.Code,
            Title = "Advanced Data Structures",
            Credits = 5,
            TeacherId = course.TeacherId
        });
        Assert.Equal(HttpStatusCode.NoContent, replaced.StatusCode);

        using var deleted = await client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        using var missing = await client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.StartsWith(
            "application/problem+json",
            missing.Content.Headers.ContentType?.ToString(),
            StringComparison.OrdinalIgnoreCase);

        using var problem = JsonDocument.Parse(await missing.Content.ReadAsStringAsync());
        Assert.True(problem.RootElement.TryGetProperty("title", out _));
    }
}
