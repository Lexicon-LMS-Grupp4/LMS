namespace LMS.Shared.DTOs.ActivityType;

public record CreateActivityTypeDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}