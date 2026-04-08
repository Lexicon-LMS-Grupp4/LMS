namespace Domain.Models.Entities;

public class Module : EntityBase
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Icon { get; set; }
    public ICollection<Activity> Activities { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
}
