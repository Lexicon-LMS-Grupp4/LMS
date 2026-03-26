namespace LMS.Shared.DTOs.User;

/// <summary>
/// Student row for classmates list (Id, display name, email).
/// </summary>
public sealed class StudentDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}
