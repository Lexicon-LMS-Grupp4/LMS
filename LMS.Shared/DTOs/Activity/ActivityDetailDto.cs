using System.Collections.Generic;
using LMS.Shared.DTOs.Document;
using LMS.Shared.DTOs.Submission;

namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Extended activity information including related entities.
    /// Used for retrieving detailed activity view with documents and submissions.
    /// Inherits basic activity properties from ActivityDto.
    /// </summary>
    public record ActivityDetailDto : ActivityDto
    {
        /// <summary>
        /// List of documents attached to this activity.
        /// Populated from the DOCUMENT table where ActivityId matches this activity.
        /// </summary>
        public List<DocumentDto> Documents { get; set; } = new();

        /// <summary>
        /// List of student submissions for this activity.
        /// Populated from the SUBMISSION table where ActivityId matches this activity.
        /// </summary>
        public List<SubmissionDto> Submissions { get; set; } = new();
    }
}