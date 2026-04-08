namespace LMS.Shared.DTOs.Document
{
    /// <summary>
    /// Represents a document attached to a course, module, or activity.
    /// Documents can be associated with one of three scopes: Course, Module, or Activity.
    /// </summary>
    public record DocumentDto
    {
        public required int Id { get; init; }
        /// <summary>
        /// Gets or sets the URL endpoint of the associated file.
        /// </summary>
        public string? FileUrl { get; init; }
        public long FileSize { get; init; }
        public required string DisplayName { get; init; }
        public required string Description { get; init; }

        /// <summary>
        /// Date and time when the document was uploaded.
        /// </summary>
        public required DateTime UploadedAt { get; init; }

        /// <summary>
        /// User ID of the person who uploaded the document.
        /// References APPLICATIONUSER.Id.
        /// </summary>
        public required string UploadedByUserId { get; init; }

        /// <summary>
        /// Name of the user who uploaded the document.
        /// Populated by joining with APPLICATIONUSER table.
        /// </summary>
        public required string UploadedByUserName { get; init; }
        public int? CourseId { get; init; }
        public int? ModuleId { get; init; }
        public int? ActivityId { get; init; }
        public int? SubmissionId { get; init; }
        public required string Scope { get; init; }
    }
}