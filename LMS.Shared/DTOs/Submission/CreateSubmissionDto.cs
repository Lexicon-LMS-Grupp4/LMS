using Microsoft.AspNetCore.Http;

namespace LMS.Shared.DTOs.Submission;

/// <summary>
/// Data transfer object for creating a new submission.
/// Used in POST /api/submissions requests.
/// TODO: add file
/// </summary>
public class CreateSubmissionDto
{
    public int ActivityId { get; set; }
    public string? Body { get; set; }
    public IFormFile? File { get; set; }
    public string? FileDescription { get; set; }
    public bool RemoveFile { get; set; }
}
