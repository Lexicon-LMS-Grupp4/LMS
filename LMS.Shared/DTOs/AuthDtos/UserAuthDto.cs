using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.AuthDtos;

public record UserAuthDto
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
