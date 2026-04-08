namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Data transfer object for submitting feedback on a student submission.
/// Used in POST /api/submissions/{id}/feedback requests.
/// Only teachers can provide feedback.
/// </summary>
public record SubmitCommentDto
{
    public required string CommentText { get; init; }
}