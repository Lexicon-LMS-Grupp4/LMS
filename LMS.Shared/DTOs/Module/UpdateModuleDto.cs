namespace LMS.Shared.DTOs.Module;

/// <summary>
/// Data transfer object for updating an existing module.
/// Used in PUT /api/modules/{id} requests.
/// </summary>
public record UpdateModuleDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Icon { get; init; }
}