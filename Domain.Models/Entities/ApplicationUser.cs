using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public int? CourseId { get; set; }
    public Course? Course { get; set; }
    public ICollection<CourseTeacher> TeachingCourses { get; set; } = [];
    public ICollection<Document> UploadedDocuments { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
    public ICollection<SubmissionComment> SubmissionComments { get; set; } = [];

}
