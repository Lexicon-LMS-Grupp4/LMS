namespace Domain.Models.Entities;

public class Submission : EntityBase
{
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;
    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
    public string? Body { get; set; }
    public Document? Document { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsLate { get; set; }
    public ICollection<SubmissionComment> Comments { get; set; } = [];

}
