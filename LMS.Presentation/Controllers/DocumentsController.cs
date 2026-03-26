using LMS.Shared.DTOs.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/documents")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public DocumentsController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all documents",
        Description = "Retrieves a paginated list of all documents, optionally filtered by course"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully")]
    public async Task<IActionResult> GetAllDocuments([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? courseId = null)
    {
        var result = await serviceManager.DocumentService.GetAllDocumentsAsync(page, pageSize, courseId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get document by ID",
        Description = "Retrieves a specific document by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Document retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        var document = await serviceManager.DocumentService.GetDocumentByIdAsync(id);
        if (document == null)
            return NotFound(new { message = "Document not found" });

        return Ok(document);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new document",
        Description = "Uploads a new document with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Document created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDto dto)
    {
        var document = await serviceManager.DocumentService.CreateDocumentAsync(dto);
        return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id }, document);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update a document",
        Description = "Updates an existing document's metadata"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Document updated successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto dto)
    {
        await serviceManager.DocumentService.UpdateDocumentAsync(id, dto);
        return Ok(new { message = "Document updated successfully" });
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a document",
        Description = "Deletes a document by its ID"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Document deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        await serviceManager.DocumentService.DeleteDocumentAsync(id);
        return Ok(new { message = "Document deleted successfully" });
    }
}