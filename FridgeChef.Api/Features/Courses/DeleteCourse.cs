namespace FridgeChef.Api.Features.Courses;

public static class DeleteCourse
{
    public static void MapEndpoint(IEndpointRouteBuilder routes)
    {
        routes.MapDelete("/{id:int}", (int id, CourseStore store) =>
                store.Remove(id)
                    ? Results.NoContent()
                    : Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Course not found",
                        detail: $"Course with ID {id} was not found."))
            .WithName("DeleteCourse")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
