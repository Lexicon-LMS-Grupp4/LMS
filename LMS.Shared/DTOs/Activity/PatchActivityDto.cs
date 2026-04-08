using Domain.Models.Enums;
using System;

namespace LMS.Shared.DTOs.Activity
{
    /// <summary>
    /// Data transfer object for partially updating an existing activity.
    /// Used in PATCH /api/activities/{id} requests.
    /// </summary>
    public record PatchActivityDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ActivityType? Type { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}