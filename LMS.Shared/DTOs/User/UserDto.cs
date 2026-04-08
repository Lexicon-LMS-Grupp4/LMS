namespace LMS.Shared.DTOs.User;

public record UserDto : UserBasicDto
{
    public required string Role { get; init; }
    public int? CourseId { get; init; }
    public string? CourseName { get; init; }
}
