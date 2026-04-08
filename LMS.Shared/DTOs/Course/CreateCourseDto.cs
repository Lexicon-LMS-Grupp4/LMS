namespace LMS.Shared.DTOs.Course;

/// <summary>
/// Data transfer object for creating a new course.
/// Used in POST /api/courses requests.
/// </summary>
public record CreateCourseDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTime StartDate { get; init; }
}
