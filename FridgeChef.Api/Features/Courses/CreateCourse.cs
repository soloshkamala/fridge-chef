namespace FridgeChef.Api.Features.Courses;

public static class CreateCourse
{
    public static void MapEndpoint(IEndpointRouteBuilder routes)
    {
        routes.MapPost("/", (CreateCourseRequest request, CourseStore store) =>
            {
                var errors = CourseRequestValidation.Validate(request);
                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                var course = new Course(0, request.Code, request.Title, request.Credits, request.TeacherId);
                var saved = store.TrySave(course);
                if (saved is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status409Conflict,
                        title: "Course code already exists",
                        detail: $"A course with code '{request.Code}' already exists.");
                }

                return Results.CreatedAtRoute(
                    "GetCourse",
                    new { id = saved.Id },
                    CourseResponse.FromCourse(saved));
            })
            .WithName("CreateCourse")
            .Produces<CourseResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
