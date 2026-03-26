namespace Domain.Models.Entities;

public class Course : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public ICollection<Module> Modules { get; set; } = [];
    public ICollection<ApplicationUser> Students { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<CourseTeacher> CourseTeachers { get; set; } = [];
}
