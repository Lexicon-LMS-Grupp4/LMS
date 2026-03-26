namespace LMS.Shared.DTOs.Module
{
    /// <summary>
    /// Represents a module with basic information and computed activity count.
    /// Used for listing modules and retrieving single module details.
    /// </summary>
    public record ModuleDto
    {
        public required int Id { get; set; }
        public required int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Total number of activities in this module.
        /// Computed by counting ACTIVITY records with matching ModuleId.
        /// </summary>
        public int ActivityCount { get; set; }
    }
}