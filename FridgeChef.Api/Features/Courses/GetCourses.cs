namespace FridgeChef.Api.Features.Courses;

public static class GetCourses
{
    public static void MapEndpoint(IEndpointRouteBuilder routes)
    {
        routes.MapGet("/", (CourseStore store) =>
                Results.Ok(store.GetAll().Select(CourseResponse.FromCourse).ToArray()))
            .WithName("GetCourses")
            .Produces<CourseResponse[]>(StatusCodes.Status200OK);
    }
}
