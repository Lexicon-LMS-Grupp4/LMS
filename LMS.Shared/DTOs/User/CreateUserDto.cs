namespace LMS.Shared.DTOs.User;

public record CreateUserDto
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string Role { get; init; } = "Student";
    public int? CourseId { get; init; }
}
