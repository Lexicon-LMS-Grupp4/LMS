namespace LMS.Shared.DTOs.User;

public record UpdateUserDto
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Role { get; init; }
    public int? CourseId { get; init; }
}
