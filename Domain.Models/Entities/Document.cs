namespace Domain.Models.Entities;

public class Document : EntityBase
{
    /// <summary>
    /// A Guid-based filename used for storing the file on the server, ensuring uniqueness and preventing conflicts.
    /// </summary>
    public string StoredFileName { get; set; } = string.Empty;
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    /// <summary>
    /// Original filename as uploaded by the user, used for display purposes and to preserve the original file name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>
    /// User provided description of the document, which can be used to give additional context or information about the file. This field is optional and can be left empty if not needed.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public ApplicationUser UploadedByUser { get; set; } = null!;
    public int? CourseId { get; set; } = null;
    public Course? Course { get; set; }
    public int? ModuleId { get; set; } = null;
    public Module? Module { get; set; }
    public int? ActivityId { get; set; } = null;
    public Activity? Activity { get; set; }
    public int? SubmissionId { get; set; } = null;
    public Submission? Submission { get; set; }
}
