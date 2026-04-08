namespace LMS.Shared.DTOs.Module;

/// <summary>
/// Data transfer object for partially updating an existing module.
/// Used in PATCH /api/modules/{id} requests.
/// </summary>
public record PatchModuleDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Icon { get; init; }
}