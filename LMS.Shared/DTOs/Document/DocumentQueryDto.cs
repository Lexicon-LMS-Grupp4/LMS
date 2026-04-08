namespace LMS.Shared.DTOs.Document;

public record DocumentQueryDto
{
    public int? CourseId { get; init; }
    public int? ModuleId { get; init; }
    public int? ActivityId { get; init; }
    public int? SubmissionId { get; init; }
}