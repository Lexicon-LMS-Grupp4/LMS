namespace LMS.Shared.DTOs.Course;

/// <summary>
/// Represents a course with basic information and computed counts.
/// Used for listing courses and retrieving single course details.
/// </summary>
public record CourseDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTime StartDate { get; init; }

    /// <summary>
    /// List of teacher user IDs assigned to this course.
    /// Populated from the COURSETEACHER junction table.
    /// </summary>
    public required List<String> TeacherIds { get; init; } = new();

    /// <summary>
    /// Total number of students enrolled in this course.
    /// Computed by counting APPLICATIONUSER records with matching CourseId.
    /// </summary>
    public required int StudentCount { get; init; }

    /// <summary>
    /// Total number of modules in this course.
    /// Computed by counting MODULE records with matching CourseId.
    /// </summary>
    public required int ModuleCount { get; init; }

}
