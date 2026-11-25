using System.Text.Json;

namespace Server.Tests;

public sealed class OpenApiTests
{
    [Theory]
    [InlineData("/api/courses", "get", "200")]
    [InlineData("/api/courses", "post", "201,400,409")]
    [InlineData("/api/courses/{id}", "get", "200,404")]
    [InlineData("/api/courses/{id}", "put", "204,400,404,409")]
    [InlineData("/api/courses/{id}", "delete", "204,404")]
    public async Task OpenApiDocument_ListsExpectedResponseCodes(
        string path,
        string method,
        string expectedCodes)
    {
        using var factory = new ApiFactory
        {
            UseTestAuthentication = true
        };

        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/openapi/v1.json");

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            $"OpenAPI returned {response.StatusCode}: {body}");

        using var document = JsonDocument.Parse(body);

        var responses = document.RootElement
            .GetProperty("paths")
            .GetProperty(path)
            .GetProperty(method)
            .GetProperty("responses");

        var actualCodes = responses.EnumerateObject()
            .Select(response => response.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(
            expectedCodes.Split(',').ToHashSet(StringComparer.Ordinal),
            actualCodes);
    }
}