namespace LMS.Shared.DTOs.User;

/// <summary>
/// Extended user information for students.
/// Includes course enrollment details in addition to basic user information.
/// Inherits from UserBasicDto.
/// </summary>
public record StudentBasicDto : UserBasicDto
{
    public int? CourseId { get; init; }
    public string? CourseName { get; init; }
}