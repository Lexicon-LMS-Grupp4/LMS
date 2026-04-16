using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.User;

public record CreateUserDto
{
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string? Name { get; init; }

    [EmailAddress]
    [MaxLength(254, ErrorMessage = "Email cannot exceed 254 characters.")]
    public string? Email { get; init; }

    public string Role { get; init; } = string.Empty;
    public int? CourseId { get; init; }

    public string Password { get; init; } = string.Empty;
}
