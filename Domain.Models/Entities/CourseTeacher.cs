namespace Domain.Models.Entities;

public class CourseTeacher : EntityBase
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser Teacher { get; set; } = null!;
}
