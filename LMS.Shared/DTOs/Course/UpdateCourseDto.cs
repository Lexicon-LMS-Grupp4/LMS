using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Course
{
    /// <summary>
    /// Data transfer object for updating an existing course.
    /// Used in PUT /api/courses/{id} requests.
    /// All properties are nullable to support partial updates.
    /// </summary>
    public record UpdateCourseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
