using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.User;

namespace LMS.Shared.DTOs.Course;

/// <summary>
/// Extended course information including related entities.
/// Used for retrieving detailed course view with modules and users.
/// Inherits basic course properties from CourseDto.
/// </summary>
public record CourseDetailDto : CourseDto
{
    /// <summary>
    /// List of modules belonging to this course.
    /// Populated from the MODULE table where ModuleId matches this course.
    /// </summary>
    public List<StudentBasicDto> Students { get; init; } = new();
    /// <summary>
    /// List of students enrolled in this course.
    /// Populated from APPLICATIONUSER table where CourseId matches this course.
    /// </summary>
    public List<ModuleDto> Modules { get; init; } = new();
}
