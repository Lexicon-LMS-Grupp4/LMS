using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Document
{
    /// <summary>
    /// Data transfer object for creating a new document.
    /// Used in POST /api/documents requests.
    /// Must specify exactly one of CourseId, ModuleId, or ActivityId.
    /// </summary>
    public class CreateDocumentDto
    {
        [MaxLength(150, ErrorMessage = "Description max length is 150 characters")]
        public string Description { get; init; } = string.Empty;
        public IFormFile File { get; init; } = null!;
        public int? CourseId { get; init; }
        public int? ModuleId { get; init; }
        public int? ActivityId { get; init; }
        public int? SubmissionId { get; init; }
    }
}