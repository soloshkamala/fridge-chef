using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Api.Features.Courses;

internal static class CourseRequestValidation
{
    public static Dictionary<string, string[]> Validate(CreateCourseRequest request)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        return validationResults
            .SelectMany(result =>
                (result.MemberNames.Any() ? result.MemberNames : [string.Empty])
                .Select(member => new { Member = member, result.ErrorMessage }))
            .GroupBy(error => error.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage ?? "The value is invalid.").ToArray());
    }
}
