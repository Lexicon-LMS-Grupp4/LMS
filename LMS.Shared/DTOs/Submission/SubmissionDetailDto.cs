using LMS.Shared.DTOs.SubmissionComment;

namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Represents a student's submission for an activity.
/// Includes submission content, timing, and optional feedback.
/// </summary>
public record SubmissionDetailDto : SubmissionDto
{
    public List<SubmissionCommentDto> Comments { get; init; } = [];

}