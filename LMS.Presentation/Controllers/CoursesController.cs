using LMS.Shared.DTOs.Course;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/courses")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public CoursesController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all courses",
        Description = "Retrieves a paginated list of all courses"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully")]
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await serviceManager.CourseService.GetAllCoursesAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get course by ID",
        Description = "Retrieves a specific course by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Course retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var course = await serviceManager.CourseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        return Ok(course);
    }

    [HttpGet("{id}/detail")]
    [SwaggerOperation(
        Summary = "Get course details",
        Description = "Retrieves detailed course information including modules and students"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Course details retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> GetCourseDetail(int id)
    {
        var course = await serviceManager.CourseService.GetCourseDetailByIdAsync(id);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        return Ok(course);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new course",
        Description = "Creates a new course with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Course created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var course = await serviceManager.CourseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a course",
        Description = "Updates an existing course with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Course updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
    {
        await serviceManager.CourseService.UpdateCourseAsync(id, dto);
        return Ok(new { message = "Course updated successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a course",
        Description = "Deletes a course by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Course deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        await serviceManager.CourseService.DeleteCourseAsync(id);
        return Ok(new { message = "Course deleted successfully" });
    }
}