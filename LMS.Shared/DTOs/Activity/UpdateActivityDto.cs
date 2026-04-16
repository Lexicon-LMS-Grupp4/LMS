namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Data transfer object for updating an existing activity.
    /// Used in PUT /api/activities/{id} requests.
    /// </summary>
    public record UpdateActivityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ActivityTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}