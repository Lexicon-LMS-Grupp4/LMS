using LMS.Shared.DTOs.Activity;
using LMS.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/activities")]
[ApiController]
[Authorize]
public class ActivitiesController : LmsControllerBase
{
    public ActivitiesController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all activities",
        Description = "Retrieves a paginated list of all activities that the user has access to, optionally filtered by module"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activities retrieved successfully")]
    public async Task<IActionResult> GetAllActivities([FromQuery] ActivitiesRequestParams query)
    {
        var result = await serviceManager.ActivityService.GetAllActivitiesAsync(UserId, query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get activity by ID",
        Description = "Retrieves a specific activity by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetActivityById(int id)
    {
        var activity = await serviceManager.ActivityService.GetActivityByIdAsync(id, UserId);
        return Ok(activity);
    }

    [HttpGet("{id}/detail")]
    [SwaggerOperation(
        Summary = "Get activity details",
        Description = "Retrieves detailed activity information including documents and submissions"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity details retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetActivityDetail(int id)
    {
        var activity = await serviceManager.ActivityService.GetActivityDetailByIdAsync(id, UserId);
        return Ok(activity);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new activity",
        Description = "Creates a new activity with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Activity created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
    {
        var activity = await serviceManager.ActivityService.CreateActivityAsync(UserId, dto);
        return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update an activity",
        Description = "Updates an existing activity with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
    {
        await serviceManager.ActivityService.UpdateActivityAsync(id, UserId, dto);
        return Ok(new { message = "Activity updated successfully" });
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(
        Summary = "Partially update an activity.",
        Description = "Partially updates an existing activity with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> PatchActivity(int id, [FromBody] PatchActivityDto dto)
    {
        await serviceManager.ActivityService.PatchActivityAsync(id, UserId, dto);
        return Ok(new { message = "Activity patched successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete an activity",
        Description = "Deletes an activity by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        await serviceManager.ActivityService.DeleteActivityAsync(id, UserId);
        return Ok(new { message = "Activity deleted successfully" });
    }
}