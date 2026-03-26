namespace LMS.Shared.DTOs.Module
{
    /// <summary>
    /// Data transfer object for partially updating an existing module.
    /// Used in PATCH /api/modules/{id} requests.
    /// </summary>
    public record PatchModuleDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}