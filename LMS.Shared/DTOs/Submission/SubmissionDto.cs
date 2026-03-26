using System;
using LMS.Shared.DTOs.Document;

namespace LMS.Shared.DTOs.Submission
{
    /// <summary>
    /// Represents a student's submission for an activity.
    /// Includes submission content, timing, and optional feedback.
    /// </summary>
    public record SubmissionDto
    {
        public required int Id { get; set; }
        public required string StudentId { get; set; }
        public required string StudentName { get; set; }
        public required int ActivityId { get; set; }
        public required string ActivityName { get; set; }
        public string? Body { get; set; }
        public int? DocumentId { get; set; }

        /// <summary>
        /// Details of the attached document, if any.
        /// Null if DocumentId is null.
        /// </summary>
        public DocumentDto? Document { get; set; }
        public required DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Indicates whether the submission was submitted after the activity deadline.
        /// </summary>
        public required bool IsLate { get; set; }
        public string? FeedbackText { get; set; }
        public DateTime? FeedbackGivenAt { get; set; }
    }
}