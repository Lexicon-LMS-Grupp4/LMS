namespace LMS.Shared.DTOs.Document
{
    /// <summary>
    /// Data transfer object for creating a new document.
    /// Used in POST /api/documents requests.
    /// Must specify exactly one of CourseId, ModuleId, or ActivityId.
    /// </summary>
    public record CreateDocumentDto
    {
        public required string FileName { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }

        /// <summary>
        /// User ID of the person uploading the document.
        /// Must reference an existing user.
        /// </summary>
        public required string UploadedByUserId { get; set; }
        public int? CourseId { get; set; } = null;
        public int? ModuleId { get; set; } = null;
        public int? ActivityId { get; set; } = null;
    }
}