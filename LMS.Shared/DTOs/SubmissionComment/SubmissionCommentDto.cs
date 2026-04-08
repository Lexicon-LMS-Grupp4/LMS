namespace LMS.Shared.DTOs.SubmissionComment;

public record SubmissionCommentDto : SubmissionCommentBaseDto
{
    public int Id { get; init; }
    public string AuthorId { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

}