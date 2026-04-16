namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Data transfer object for creating a new activity.
    /// Used in POST /api/activities requests.
    /// </summary>
    public record CreateActivityDto
    {
        public required int ModuleId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int ActivityTypeId { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }
}