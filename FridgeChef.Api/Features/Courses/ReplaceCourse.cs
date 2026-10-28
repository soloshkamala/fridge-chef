namespace FridgeChef.Api.Features.Courses;

public static class ReplaceCourse
{
    public static void MapEndpoint(IEndpointRouteBuilder routes)
    {
        routes.MapPut("/{id:int}", (int id, CreateCourseRequest request, CourseStore store) =>
            {
                var errors = CourseRequestValidation.Validate(request);
                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                var replacement = new Course(id, request.Code, request.Title, request.Credits, request.TeacherId);
                if (!store.TryReplace(replacement, out var codeConflict))
                {
                    return codeConflict
                        ? Results.Problem(
                            statusCode: StatusCodes.Status409Conflict,
                            title: "Course code already exists",
                            detail: $"A course with code '{request.Code}' already exists.")
                        : Results.Problem(
                            statusCode: StatusCodes.Status404NotFound,
                            title: "Course not found",
                            detail: $"Course with ID {id} was not found.");
                }

                return Results.NoContent();
            })
            .WithName("ReplaceCourse")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
