using Domain.Models.Exceptions;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET api/users?page=1&pageSize=10
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync(id, cancellationToken);
        return user is null ? NotFound($"User '{id}' was not found.") : Ok(user);
    }

    // GET api/users/course/{courseId}
    [Authorize(Roles = "Teacher,Admin,Student")]
    [HttpGet("course/{courseId:int}")]
    [SwaggerOperation(Summary = "Get students enrolled in a course")]
    public async Task<IActionResult> GetUsersByCourse(int courseId, CancellationToken cancellationToken)
    {
        //var result = await _serviceManager.UserService.GetUsersByCourseAsync(courseId, cancellationToken);
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var classmates = await _serviceManager.UserService.GetUsersByCourseAsync(courseId, cancellationToken);
        var result = classmates?.Where(u => u.Id != currentUserId).ToList();
        return Ok(result);
    }

    // GET api/users/without-course?page=1&pageSize=10
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
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

    // POST api/users
    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a user")]
    [SwaggerResponse(201, "Created successfully")]
    [SwaggerResponse(400, "Failed to create user")]
    public async Task<IActionResult> CreateUser(CreateUserDto userDto, CancellationToken cancellationToken)
    {
        var (result, user) = await _serviceManager.UserService.CreateUserAsync(userDto, cancellationToken);
        // couldnt throw bc limitations in <Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory>.CreateProblemDetails() (used in /LMS.API/Extensions/ExceptionMiddlewareExtetensions.cs)
        // if(!result.Succeeded) throw new BadRequestException(message:"Failed to create user", data:result.Errors.ToDictionary(err=>err.Code, err=>err.Description));
        if (!result.Succeeded) return BadRequest(new BadRequestException(message: "Failed to create user", data: result.Errors.ToDictionary(err => err.Code, err => err.Description)));
        return Created((string?)null, user);
    }

    // PUT api/users/{userId}/enroll/{courseId}
    [Authorize(Roles = "Teacher")]
    [HttpPut("{userId}/enroll/{courseId:int}")]
    [SwaggerOperation(Summary = "Enroll a student in a course")]
    [SwaggerResponse(204, "Enrolled successfully")]
    [SwaggerResponse(404, "User or course not found")]
    public async Task<IActionResult> EnrollUserInCourse(
        string userId, int courseId, CancellationToken cancellationToken)
    {
        // TODO: handle exceptions with middleware
        try
        {
            await _serviceManager.UserService.EnrollUserInCourseAsync(userId, courseId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // PUT api/users/{userId}/remove-course
    [Authorize(Roles = "Teacher")]
    [HttpPut("{userId}/remove-course")]
    [SwaggerOperation(Summary = "Remove a student from their course")]
    [SwaggerResponse(204, "Removed successfully")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> RemoveUserFromCourse(string userId, CancellationToken cancellationToken)
    {
        // TODO: handle exceptions with middleware
        try
        {
            await _serviceManager.UserService.RemoveUserFromCourseAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // DELETE api/users/{id}
    [Authorize(Roles = "Teacher")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user permanently")]
    [SwaggerResponse(204, "Deleted successfully")]
    [SwaggerResponse(400, "Cannot delete own account")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        // TODO: handle exceptions with middleware
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

    // PUT api/users/{id}
    [Authorize(Roles = "Teacher")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update user role and course assignment")]
    [SwaggerResponse(200, "Updated successfully")]
    [SwaggerResponse(404, "User or course not found")]
    [SwaggerResponse(400, "Update failed")]
    public async Task<IActionResult> UpdateUser(
        string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _serviceManager.UserService.UpdateUserAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    // GET api/users/course/{courseId}/teachers
    [Authorize(Roles = "Teacher,Admin,Student")]
    [HttpGet("course/{courseId:int}/teachers")]
    [SwaggerOperation(Summary = "Get teachers assigned to a course")]
    public async Task<IActionResult> GetTeachersByCourse(int courseId, CancellationToken cancellationToken)
    {
        var result = await _serviceManager.UserService.GetTeachersByCourseAsync(courseId, cancellationToken);
        return Ok(result);
    }
}
