namespace LMS.Shared.DTOs.ActivityType;

public record UpdateActivityTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}