namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Data transfer object for creating a new submission.
/// Used in POST /api/submissions requests.
/// TODO: add file
/// </summary>
public record CreateSubmissionDto
{
    public required int ActivityId { get; init; }
    public string? Body { get; init; }
    public int? DocumentId { get; init; }
}