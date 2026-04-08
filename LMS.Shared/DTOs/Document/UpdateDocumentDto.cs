namespace LMS.Shared.DTOs.Document
{
    /// <summary>
    /// Data transfer object for updating an existing document's metadata.
    /// Used in PUT /api/documents/{id} requests.
    /// </summary>
    public record UpdateDocumentDto
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }
        public int? SubmissionId { get; set; }
    }
}