using Domain.Models.Enums;
using System;

namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Data transfer object for updating an existing activity.
    /// Used in PUT /api/activities/{id} requests.
    /// All properties are nullable to support partial updates.
    /// </summary>
    public record UpdateActivityDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ActivityType? Type { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}