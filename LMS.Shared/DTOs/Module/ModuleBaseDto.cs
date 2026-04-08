namespace LMS.Shared.DTOs.Module;

/// <summary>
/// Data transfer object for creating a new module.
/// Used in POST /api/modules requests.
/// </summary>
public record ModuleBaseDto
{
    public required int CourseId { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; } = string.Empty;
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public string? Icon { get; set; }

}