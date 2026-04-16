using LMS.Shared.DTOs.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/modules")]
[ApiController]
[Authorize]
public class ModulesController : LmsControllerBase
{
    public ModulesController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all modules",
        Description = "Retrieves a paginated list of all modules that the user has access to, optionally filtered by course"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Modules retrieved successfully")]
    public async Task<IActionResult> GetAllModules(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? courseId = null)
    {
        var result = await serviceManager.ModuleService.GetAllModulesAsync(UserId, page, pageSize, courseId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get module by ID",
        Description = "Retrieves a specific module by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Module retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetModuleById(int id)
    {
        var module = await serviceManager.ModuleService.GetModuleByIdAsync(id, UserId);
        return Ok(module);
    }

    [HttpGet("{id}/detail")]
    [SwaggerOperation(
        Summary = "Get module details",
        Description = "Retrieves detailed module information including activities and documents"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Module details retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetModuleDetail(int id)
    {
        var module = await serviceManager.ModuleService.GetModuleDetailByIdAsync(id, UserId);
        return Ok(module);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new module",
        Description = "Creates a new module with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Module created successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleDto dto)
    {
        var module = await serviceManager.ModuleService.CreateModuleAsync(UserId, dto);
        return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a module",
        Description = "Updates an existing module with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Module updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> UpdateModule(int id, [FromBody] UpdateModuleDto dto)
    {
        await serviceManager.ModuleService.UpdateModuleAsync(id, UserId, dto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(
        Summary = "Partially update a module",
        Description = "Updates an existing module with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Module updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> PatchModule(int id, [FromBody] PatchModuleDto dto)
    {
        await serviceManager.ModuleService.UpdateModulePartiallyAsync(id, UserId, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a module",
        Description = "Deletes a module by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Module deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> DeleteModule(int id)
    {
        await serviceManager.ModuleService.DeleteModuleAsync(id, UserId);
        return Ok(new { message = "Module deleted successfully" });
    }
}