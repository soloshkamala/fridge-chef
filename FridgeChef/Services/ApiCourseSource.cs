using System.Net.Http.Json;
using FridgeChef.Models;

namespace FridgeChef.Services;

public sealed class ApiCourseSource(HttpClient http) : ICourseSource
{
    public async Task<IReadOnlyList<Course>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        var responses = await http.GetFromJsonAsync<CourseResponse[]>(
            "api/courses",
            cancellationToken);

        if (responses is null)
        {
            throw new InvalidOperationException("The courses API returned an empty response.");
        }

        return responses
            .Select(course => new Course(
                course.Id,
                course.Code,
                course.Title,
                course.Credits,
                course.TeacherId))
            .ToArray();
    }
}
