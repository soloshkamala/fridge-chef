namespace FridgeChef.Api.Features.Courses;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourses(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/courses");

        GetCourses.MapEndpoint(group);
        GetCourse.MapEndpoint(group);
        CreateCourse.MapEndpoint(group);
        ReplaceCourse.MapEndpoint(group);
        DeleteCourse.MapEndpoint(group);

        return routes;
    }
}
