namespace LMS.Shared.DTOs.Module;

/// <summary>
/// Represents a module with basic information and computed activity count.
/// Used for listing modules and retrieving single module details.
/// </summary>
public record ModuleDto : ModuleBaseDto
{
    public required int Id { get; init; }
    public required string CourseName { get; init; }
    /// <summary>
    /// Total number of activities in this module.
    /// Computed by counting ACTIVITY records with matching ModuleId.
    /// </summary>
    public int ActivityCount { get; init; }
}