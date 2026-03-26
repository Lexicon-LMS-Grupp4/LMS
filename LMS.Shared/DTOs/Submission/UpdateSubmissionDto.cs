namespace LMS.Shared.DTOs.Submission
{
    /// <summary>
    /// Data transfer object for updating an existing submission.
    /// Used in PUT  /api/submissions/{id} requests.
    /// All properties are nullable to support partial updates.
    /// Students can only update their own submissions before the deadline.
    /// </summary>
    public record UpdateSubmissionDto
    {
        public string? Body { get; set; }
        public int? DocumentId { get; set; }
    }
}