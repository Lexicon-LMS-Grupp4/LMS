using LMS.Shared.DTOs.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/documents")]
[ApiController]
[Authorize]
public class DocumentsController : LmsControllerBase
{
    public DocumentsController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all documents",
        Description = "Retrieves a paginated list of all documents, optionally filtered by course"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Documents retrieved successfully")]
    public async Task<IActionResult> GetDocuments([FromQuery] DocumentQueryDto dto, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await serviceManager.DocumentService.GetDocumentsAsync(UserId, page, pageSize, dto);
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
        var document = await serviceManager.DocumentService.GetDocumentByIdAsync(id, UserId);
        return Ok(document);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new document",
        Description = "Uploads a new document with the provided details"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Document created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    public async Task<IActionResult> CreateDocument([FromForm] CreateDocumentDto dto)
    {
        var document = await serviceManager.DocumentService.CreateDocumentAsync(UserId, dto);
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
        var document = await serviceManager.DocumentService.UpdateDocumentAsync(id, UserId, dto);
        return Ok(document);
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
        await serviceManager.DocumentService.DeleteDocumentAsync(id, UserId);
        return Ok(new { message = "Document deleted successfully" });
    }

    [HttpGet("{id:int}/file")]
    [SwaggerOperation(
        Summary = "Download a document file",
        Description = "Returns the file associated with the specified document if the user has access."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Document retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Document not found")]
    public async Task<IActionResult> GetDocumentFile(int id, [FromQuery] bool download = false)
    {
        var file = await serviceManager.DocumentService.DownloadDocumentAsync(id, UserId);

        var canDisplayInline =
            file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
            file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

        var dispositionType = download || !canDisplayInline ? "attachment" : "inline";

        var safeFileName = Uri.EscapeDataString(file.FileDownloadName);

        Response.Headers["Content-Disposition"] =
            $"{dispositionType}; filename*=UTF-8''{safeFileName}";

        return File(file.Stream, file.ContentType);

    }
}