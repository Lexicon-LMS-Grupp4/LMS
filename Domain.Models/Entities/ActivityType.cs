namespace Domain.Models.Entities;

public class ActivityType : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Activity> Activities { get; set; } = [];
}