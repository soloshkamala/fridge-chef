namespace FridgeChef.Api.Features.Courses;

public sealed record CourseResponse(int Id, string Code, string Title, int Credits, int TeacherId)
{
    public static CourseResponse FromCourse(Course course) =>
        new(course.Id, course.Code, course.Title, course.Credits, course.TeacherId);
}
