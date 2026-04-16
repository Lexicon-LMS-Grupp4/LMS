namespace LMS.Shared.DTOs.ActivityType;

public record ActivityTypeDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}