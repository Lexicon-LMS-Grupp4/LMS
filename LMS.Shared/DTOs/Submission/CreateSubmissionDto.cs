namespace LMS.Shared.DTOs.Submission
{
    /// <summary>
    /// Data transfer object for creating a new submission.
    /// Used in POST /api/submissions requests.
    /// </summary>
    public record CreateSubmissionDto
    {
        public required string StudentId { get; set; }
        public required int ActivityId { get; set; }
        public string? Body { get; set; }
        public int? DocumentId { get; set; }
    }
}