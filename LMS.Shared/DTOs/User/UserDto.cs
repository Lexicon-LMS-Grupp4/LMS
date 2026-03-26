namespace LMS.Shared.DTOs.User;

public record UserDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public int? CourseId { get; set; }
    public string? CourseName { get; set; }
}
