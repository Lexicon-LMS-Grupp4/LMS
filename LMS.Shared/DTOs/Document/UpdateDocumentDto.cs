namespace LMS.Shared.DTOs.Document
{
    /// <summary>
    /// Data transfer object for updating an existing document's metadata.
    /// Used in PUT /api/documents/{id} requests.
    /// All properties are nullable to support partial updates.
    /// Note: FileName and scope (CourseId/ModuleId/ActivityId) cannot be changed.
    /// </summary>
    public record UpdateDocumentDto
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
    }
}