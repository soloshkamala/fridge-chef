namespace FridgeChef.Api.Features.Courses;

public sealed class CourseStore
{
    private readonly object _gate = new();
    private readonly Dictionary<int, Course> _courses = [];
    private int _nextId = 1;

    public IReadOnlyList<Course> GetAll()
    {
        lock (_gate)
        {
            return _courses.Values.ToArray();
        }
    }

    public Course? Find(int id)
    {
        lock (_gate)
        {
            return _courses.GetValueOrDefault(id);
        }
    }

    public Course? TrySave(Course course)
    {
        lock (_gate)
        {
            if (_courses.Values.Any(existing =>
                    existing.Id != course.Id &&
                    string.Equals(existing.Code, course.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var saved = course with { Id = course.Id == 0 ? _nextId++ : course.Id };
            _courses[saved.Id] = saved;
            return saved;
        }
    }

    public bool TryReplace(Course course, out bool codeConflict)
    {
        lock (_gate)
        {
            codeConflict = false;

            if (!_courses.ContainsKey(course.Id))
            {
                return false;
            }

            codeConflict = _courses.Values.Any(existing =>
                existing.Id != course.Id &&
                string.Equals(existing.Code, course.Code, StringComparison.OrdinalIgnoreCase));

            if (codeConflict)
            {
                return false;
            }

            _courses[course.Id] = course;
            return true;
        }
    }

    public bool Remove(int id)
    {
        lock (_gate)
        {
            return _courses.Remove(id);
        }
    }
}
