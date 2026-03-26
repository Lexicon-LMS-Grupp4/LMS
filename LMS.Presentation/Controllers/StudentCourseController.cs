using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[Route("api/studentcourse")]
[ApiController]
[Authorize]
public class StudentCourseController : ControllerBase
{
    private readonly IStudentCourseService studentCourseService;

    public StudentCourseController(IStudentCourseService studentCourseService)
    {
        this.studentCourseService = studentCourseService;
    }

    [HttpGet("mycourse")]
    [SwaggerOperation(
        Summary = "Get current student's enrolled course",
        Description = "Returns the course the authenticated student is enrolled in (from ApplicationUser.CourseId). Requires JWT.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course found", typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Not authenticated")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User is not a student, has no course, or course not found")]
    public async Task<ActionResult<CourseDto>> GetMyCourse()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var course = await studentCourseService.GetStudentCourseAsync(userId);
        if (course is null)
            return NotFound();

        return Ok(course);
    }

    [HttpGet("classmates")]
    [SwaggerOperation(
        Summary = "Get classmates in the same course",
        Description = "Returns other students enrolled in the same course as the current user. Current user is excluded.")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of classmates", typeof(IEnumerable<StudentDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Not authenticated")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Cannot resolve student's course")]
    public async Task<ActionResult<IReadOnlyList<StudentDto>>> GetClassmates()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var course = await studentCourseService.GetStudentCourseAsync(userId);
        if (course is null)
            return NotFound();

        var classmates = await studentCourseService.GetCourseClassmatesAsync(course.Id, userId);
        return Ok(classmates);
    }
}
