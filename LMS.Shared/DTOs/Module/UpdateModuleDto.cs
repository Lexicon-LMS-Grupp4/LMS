namespace LMS.Shared.DTOs.Module
{
    /// <summary>
    /// Data transfer object for updating an existing module.
    /// Used in PUT /api/modules/{id} requests.
    /// </summary>
    public record UpdateModuleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}