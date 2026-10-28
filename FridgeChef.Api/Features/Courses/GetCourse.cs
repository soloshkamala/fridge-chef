namespace FridgeChef.Api.Features.Courses;

public static class GetCourse
{
    public static void MapEndpoint(IEndpointRouteBuilder routes)
    {
        routes.MapGet("/{id:int}", (int id, CourseStore store) =>
            {
                var course = store.Find(id);
                return course is null
                    ? Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Course not found",
                        detail: $"Course with ID {id} was not found.")
                    : Results.Ok(CourseResponse.FromCourse(course));
            })
            .WithName("GetCourse")
            .Produces<CourseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
