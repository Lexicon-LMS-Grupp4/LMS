using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher")]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET api/users?page=1&pageSize=10
    [HttpGet]
    [SwaggerOperation(Summary = "Get all students (paged)")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _serviceManager.UserService.GetAllStudentsAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync(id, cancellationToken);
        return user is null ? NotFound($"User '{id}' was not found.") : Ok(user);
    }

    // GET api/users/course/{courseId}
    [HttpGet("course/{courseId:int}")]
    [SwaggerOperation(Summary = "Get students enrolled in a course")]
    public async Task<IActionResult> GetUsersByCourse(int courseId, CancellationToken cancellationToken)
    {
        var result = await _serviceManager.UserService.GetUsersByCourseAsync(courseId, cancellationToken);
        return Ok(result);
    }

    // GET api/users/without-course?page=1&pageSize=10
    [HttpGet("without-course")]
    [SwaggerOperation(Summary = "Get students not enrolled in any course (paged)")]
    public async Task<IActionResult> GetUsersWithoutCourse(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _serviceManager.UserService.GetUsersWithoutCourseAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    // GET api/users/teachers?page=1&pageSize=10
    [HttpGet("teachers")]
    [SwaggerOperation(Summary = "Get all teachers (paged)")]
    public async Task<IActionResult> GetTeachers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _serviceManager.UserService.GetTeachersAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    // PUT api/users/{userId}/enroll/{courseId}
    [HttpPut("{userId}/enroll/{courseId:int}")]
    [SwaggerOperation(Summary = "Enroll a student in a course")]
    [SwaggerResponse(204, "Enrolled successfully")]
    [SwaggerResponse(404, "User or course not found")]
    public async Task<IActionResult> EnrollUserInCourse(
        string userId, int courseId, CancellationToken cancellationToken)
    {
        try
        {
            await _serviceManager.UserService.EnrollUserInCourseAsync(userId, courseId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // PUT api/users/{userId}/remove-course
    [HttpPut("{userId}/remove-course")]
    [SwaggerOperation(Summary = "Remove a student from their course")]
    [SwaggerResponse(204, "Removed successfully")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> RemoveUserFromCourse(string userId, CancellationToken cancellationToken)
    {
        try
        {
            await _serviceManager.UserService.RemoveUserFromCourseAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user permanently")]
    [SwaggerResponse(204, "Deleted successfully")]
    [SwaggerResponse(400, "Cannot delete own account")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("No authenticated user.");
            await _serviceManager.UserService.DeleteUserAsync(currentUserId, id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}