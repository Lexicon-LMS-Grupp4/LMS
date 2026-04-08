namespace LMS.Shared.DTOs.SubmissionComment;

public record SubmissionCommentBaseDto
{
    public int SubmissionId { get; init; }
    public string Text { get; init; } = string.Empty;

}