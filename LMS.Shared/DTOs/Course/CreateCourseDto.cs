using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Shared.DTOs.Course
{
    /// <summary>
    /// Data transfer object for creating a new course.
    /// Used in POST /api/courses requests.
    /// </summary>
    public record CreateCourseDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required DateTime StartDate { get; set; }
    }
}
