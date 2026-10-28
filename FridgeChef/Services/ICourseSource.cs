using FridgeChef.Models;

namespace FridgeChef.Services;

public interface ICourseSource
{
    Task<IReadOnlyList<Course>> GetCoursesAsync(CancellationToken cancellationToken = default);
}
