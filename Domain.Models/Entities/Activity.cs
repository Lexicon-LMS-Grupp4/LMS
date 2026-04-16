namespace Domain.Models.Entities;

public class Activity : EntityBase
{
    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ActivityTypeId { get; set; }
    public ActivityType ActivityType { get; set; } = null!;
    public DateTime? StartTime { get; set; } // Some activities might only have an end time
    public DateTime EndTime { get; set; }
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}
