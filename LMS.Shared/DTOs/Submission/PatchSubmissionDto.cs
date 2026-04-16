using Microsoft.AspNetCore.Http;

namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Data transfer object for partially updating an existing submission.
/// Used in PATCH  /api/submissions/{id} requests.
/// Students can only update their own submissions before the deadline.
/// </summary>
public record PatchSubmissionDto
{
    public int? ActivityId { get; init; }
    public string? Body { get; init; }
    public IFormFile? File { get; init; } = null!;
    public string? FileDescription { get; init; }
}