using Domain.Models.Exceptions;
using LMS.Shared.DTOs.ActivityType;
using LMS.Shared.DTOs.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/activity-types")]
[ApiController]
public class ActivityTypesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ActivityTypesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all activity types",
        Description = "Retrieves all available activity types"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity types retrieved successfully")]
    public async Task<IActionResult> GetAllActivityTypes()
    {
        var result = await _serviceManager.ActivityTypeService.GetAllActivityTypesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get activity type by ID",
        Description = "Retrieves a specific activity type by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity type retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity type not found")]
    public async Task<IActionResult> GetActivityTypeById(int id)
    {
        var result = await _serviceManager.ActivityTypeService.GetActivityTypeByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new activity type",
        Description = "Creates a new activity type"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Activity type created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input or name already exists")]
    public async Task<IActionResult> CreateActivityType([FromBody] CreateActivityTypeDto dto)
    {
        var result = await _serviceManager.ActivityTypeService.CreateActivityTypeAsync(dto);
        return CreatedAtAction(nameof(GetActivityTypeById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update an activity type",
        Description = "Updates an existing activity type"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity type updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity type not found")]
    public async Task<IActionResult> UpdateActivityType(int id, [FromBody] UpdateActivityTypeDto dto)
    {
        await _serviceManager.ActivityTypeService.UpdateActivityTypeAsync(id, dto);
        return Ok(new { message = "Activity type updated successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete an activity type",
        Description = "Deletes an activity type by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Activity type deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Activity type not found")]
    public async Task<IActionResult> DeleteActivityType(int id)
    {
        try
        {
            await _serviceManager.ActivityTypeService.DeleteActivityTypeAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
    //public async Task<IActionResult> DeleteActivityType(int id)
    //{
    //    await _serviceManager.ActivityTypeService.DeleteActivityTypeAsync(id);
    //    return Ok(new { message = "Activity type deleted successfully" });
    //}
}