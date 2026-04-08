namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Data transfer object for updating an existing submission.
/// Used in PUT  /api/submissions/{id} requests.
/// Students can only update their own submissions before the deadline.
/// </summary>
public record UpdateSubmissionDto
{
    public int ActivityId { get; init; }
    public string Body { get; init; } = string.Empty;
    public int? DocumentId { get; init; }
}