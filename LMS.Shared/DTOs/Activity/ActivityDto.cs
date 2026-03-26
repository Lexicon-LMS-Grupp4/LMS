
using Domain.Models.Enums;

namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Represents an activity with basic information and computed counts.
    /// Used for listing activities and retrieving single activity details.
    /// </summary>
    public record ActivityDto
    {

        public required int Id { get; set; }
        public required int ModuleId { get; set; }
        public required string ModuleName { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        /// <summary>
        /// Type of activity (e.g., "Assignment", "Quiz", "Lecture", "Discussion").
        /// </summary>
        public required ActivityType Type { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }

        /// <summary>
        /// Total number of documents attached to this activity.
        /// Computed by counting DOCUMENT records with matching ActivityId.
        /// </summary>
        public int DocumentCount { get; set; }

        /// <summary>
        /// Total number of student submissions for this activity.
        /// Computed by counting SUBMISSION records with matching ActivityId.
        /// </summary>
        public int SubmissionCount { get; set; }
    }
}