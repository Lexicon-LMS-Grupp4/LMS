using LMS.Shared.DTOs.Submission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/submissions")]
[ApiController]
public class SubmissionsController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public SubmissionsController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all submissions",
        Description = "Retrieves a paginated list of submissions, optionally filtered by activity or student"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submissions retrieved successfully")]
    public async Task<IActionResult> GetAllSubmissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? activityId = null,
        [FromQuery] string? studentId = null)
    {
        var result = await serviceManager.SubmissionService.GetAllSubmissionsAsync(page, pageSize, activityId, studentId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get submission by ID",
        Description = "Retrieves a specific submission by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    public async Task<IActionResult> GetSubmissionById(int id)
    {
        var submission = await serviceManager.SubmissionService.GetSubmissionByIdAsync(id);
        if (submission == null)
            return NotFound(new { message = "Submission not found" });

        return Ok(submission);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new submission",
        Description = "Creates a new student submission for an activity"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Submission created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<IActionResult> CreateSubmission([FromBody] CreateSubmissionDto dto)
    {
        var submission = await serviceManager.SubmissionService.CreateSubmissionAsync(dto);
        return CreatedAtAction(nameof(GetSubmissionById), new { id = submission.Id }, submission);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a submission",
        Description = "Updates an existing submission"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    public async Task<IActionResult> UpdateSubmission(int id, [FromBody] UpdateSubmissionDto dto)
    {
        await serviceManager.SubmissionService.UpdateSubmissionAsync(id, dto);
        return Ok(new { message = "Submission updated successfully" });
    }

    [HttpPost("{id}/feedback")]
    [SwaggerOperation(
        Summary = "Submit feedback for a submission",
        Description = "Provides teacher feedback on a student submission"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Feedback submitted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    public async Task<IActionResult> SubmitFeedback(int id, [FromBody] SubmitFeedbackDto dto)
    {
        await serviceManager.SubmissionService.SubmitFeedbackAsync(id, dto);
        return Ok(new { message = "Feedback submitted successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a submission",
        Description = "Deletes a submission by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found")]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        await serviceManager.SubmissionService.DeleteSubmissionAsync(id);
        return Ok(new { message = "Submission deleted successfully" });
    }
}