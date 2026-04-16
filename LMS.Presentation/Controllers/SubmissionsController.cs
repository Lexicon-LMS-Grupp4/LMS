using LMS.Shared.DTOs.Submission;
using LMS.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/submissions")]
[ApiController]
[Authorize]
public class SubmissionsController : LmsControllerBase
{
    private readonly ILogger<SubmissionsController> _logger;
    public SubmissionsController(IServiceManager serviceManager, ILogger<SubmissionsController> logger) : base(serviceManager)
    {
        this._logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all submissions",
        Description = "Retrieves a paginated list of submissions that the user has access to, optionally filtered by activity or student"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submissions retrieved successfully")]
    public async Task<IActionResult> GetAllSubmissions(
        [FromQuery] SubmissionsRequestParams query)
    {
        var result = await serviceManager.SubmissionService.GetAllSubmissionsAsync(UserId, query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get submission by ID",
        Description = "Retrieves a specific submission by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetSubmissionById(int id)
    {
        var submission = await serviceManager.SubmissionService.GetSubmissionByIdAsync(id, UserId);
        return Ok(submission);
    }

    [HttpGet("{id}/detail")]
    [SwaggerOperation(
        Summary = "Get submission by ID",
        Description = "Retrieves a specific submission by its ID including its comments"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> GetSubmissionDetailsById(int id)
    {
        var submission = await serviceManager.SubmissionService.GetSubmissionDetailByIdAsync(id, UserId);
        return Ok(submission);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new submission",
        Description = "Creates a new student submission for an activity"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Submission created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]

    public async Task<IActionResult> CreateSubmission([FromForm] CreateSubmissionDto dto)
    {
        var result = await serviceManager.SubmissionService.CreateSubmissionAsync(UserId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a submission",
        Description = "Updates an existing submission"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> UpdateSubmission(int id, [FromForm] UpdateSubmissionDto dto)
    {
        await serviceManager.SubmissionService.UpdateSubmissionAsync(id, UserId, dto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(
        Summary = "Partially update a submission",
        Description = "Updates an existing submission"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> PatchSubmission(int id, [FromForm] PatchSubmissionDto dto)
    {
        await serviceManager.SubmissionService.UpdateSubmissionPartiallyAsync(id, UserId, dto);
        return NoContent();
    }

    [HttpPost("{id}/comment")]
    [SwaggerOperation(
        Summary = "Submit feedback for a submission",
        Description = "Provides teacher feedback on a student submission"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Feedback submitted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> SubmitComment(int id, [FromBody] SubmitCommentDto dto)
    {
        await serviceManager.SubmissionService.SubmitCommentAsync(id, UserId, dto);
        return Ok(new { message = "Feedback submitted successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a submission",
        Description = "Deletes a submission by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        await serviceManager.SubmissionService.DeleteSubmissionAsync(id, UserId);
        return Ok(new { message = "Submission deleted successfully" });
    }
}