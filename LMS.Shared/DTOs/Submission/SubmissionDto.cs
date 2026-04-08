using LMS.Shared.DTOs.Document;

namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Represents a student's submission for an activity.
/// Includes submission content, timing, and optional feedback.
/// </summary>
public record SubmissionDto
{
    public required int Id { get; init; }
    public required string StudentId { get; init; }
    public required string StudentName { get; init; }
    public required int ActivityId { get; init; }
    public required string ActivityName { get; init; }
    public string Body { get; init; } = string.Empty;
    public int? DocumentId { get; init; }

    /// <summary>
    /// Details of the attached document, if any.
    /// Null if DocumentId is null.
    /// </summary>
    public DocumentDto? Document { get; init; }
    public required DateTime SubmittedAt { get; init; }

    /// <summary>
    /// Indicates whether the submission was submitted after the activity deadline.
    /// </summary>
    public required bool IsLate { get; init; }
    public int CommentCount { get; init; }
}