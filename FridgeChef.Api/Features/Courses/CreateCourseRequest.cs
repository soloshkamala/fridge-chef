using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Api.Features.Courses;

public sealed class CreateCourseRequest
{
    [Required]
    [StringLength(10)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Title { get; init; } = string.Empty;

    [Range(1, 30)]
    public int Credits { get; init; }

    [Range(1, int.MaxValue)]
    public int TeacherId { get; init; }
}
