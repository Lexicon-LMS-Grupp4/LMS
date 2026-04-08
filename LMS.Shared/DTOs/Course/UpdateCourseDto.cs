namespace LMS.Shared.DTOs.Course;

/// <summary>
/// Data transfer object for updating an existing course.
/// Used in PUT /api/courses/{id} requests.
/// All properties are nullable to support partial updates.
/// </summary>
public record UpdateCourseDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
}
