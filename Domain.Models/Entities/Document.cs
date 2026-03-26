namespace Domain.Models.Entities;

public class Document : EntityBase
{
    public string FileName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
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
