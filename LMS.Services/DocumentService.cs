using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Services.Access;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Document;
using Service.Contracts;

namespace LMS.Services;
/// <summary>
/// Provides operations for managing documents, including retrieval, creation, update, deletion,
/// and access control based on user roles and course relationships.
/// </summary>
public class DocumentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IDocumentManager documentManager,
    ILmsRelationResolver lmsRelationResolver,
    ILmsAccessService lmsAccessService,
    IUserAccessContextFactory userAccessContextFactory) : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILmsRelationResolver _lmsRelationResolver = lmsRelationResolver;
    private readonly ILmsAccessService _lmsAccessService = lmsAccessService;
    private readonly IUserAccessContextFactory _userAccessContextFactory = userAccessContextFactory;
    private readonly IDocumentManager _documentManager = documentManager;

    /// <summary>
    /// Retrieves a paged list of documents accessible to the specified user.
    /// </summary>
    /// <param name="userId">The user requesting the documents.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="dto">Filter criteria for the document query.</param>
    /// <returns>A paged result containing accessible documents.</returns>
    public async Task<PagedResultDto<DocumentDto>> GetDocumentsAsync(string userId, int page, int pageSize, DocumentQueryDto dto)
    {

        // TODO: use CancellationToken
        var access = await _userAccessContextFactory.CreateAsync(userId);

        var query = _lmsAccessService.ApplyDocumentAccessFilter(
            _unitOfWork.Documents.BuildBasicQuery(dto),
            access);

        var (documents, totalCount) = await query
            .OrderByDescending(d => d.UploadedAt)
            .PagedResult(page, pageSize);

        var dtos = _mapper.Map<List<DocumentDto>>(documents);

        return new PagedResultDto<DocumentDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Retrieves a document by its identifier if the user has access.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="userId">The user requesting the document.</param>
    /// <returns>The document if accessible.</returns>
    /// <exception cref="NotFoundException">
    /// Thrown if the document does not exist or the user is not authorized to access it.
    /// </exception>
    public async Task<DocumentDto> GetDocumentByIdAsync(int id, string userId)
    {
        var document = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(id, false)
            ?? throw new NotFoundException($"Document with id {id} does not exist");

        // TODO add cancellation token
        await EnsureDocumentExistsAndAccessibleAsync(userId, document);

        return _mapper.Map<DocumentDto>(document);
    }

    /// <summary>
    /// Retrieves a document file if the user has access.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="userId">The user requesting the file.</param>
    /// <returns>The file stream and metadata.</returns>
    /// <exception cref="NotFoundException">
    /// Thrown if the document or file does not exist, or the user is not authorized.
    /// </exception>
    public async Task<DocumentDownloadDto> DownloadDocumentAsync(int id, string userId)
    {
        var document = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(id)
            ?? throw new NotFoundException($"Document with id {id} does not exist");

        // TODO add cancellation token
        await EnsureDocumentExistsAndAccessibleAsync(userId, document);

        if (string.IsNullOrWhiteSpace(document.StoredFileName))
        {
            throw new NotFoundException("File not found.");
        }

        var stream = await _documentManager.OpenReadAsync(document);

        var contentType = string.IsNullOrWhiteSpace(document.ContentType)
            ? "application/octet-stream"
            : document.ContentType;

        return new DocumentDownloadDto
        {
            Stream = stream,
            ContentType = contentType,
            FileDownloadName = document.DisplayName ?? "download"
        };
    }

    /// <summary>
    /// Creates a new document and stores its associated file.
    /// </summary>
    /// <param name="userId">The user creating the document.</param>
    /// <param name="dto">The document creation data.</param>
    /// <returns>The created document.</returns>
    /// <exception cref="BadRequestException">
    /// Thrown if the file is missing or no ownership relation is specified.
    /// </exception>
    public async Task<DocumentDto> CreateDocumentAsync(string userId, CreateDocumentDto dto)
    {
        if (dto.File == null)
        {
            throw new BadRequestException("File is missing.");
        }

        await ValidateCreateAccessAsync(userId, dto);

        var document = await _documentManager.CreateEntityAsync(
            userId,
            courseId: dto.CourseId,
            moduleId: dto.ModuleId,
            activityId: dto.ActivityId,
            submissionId: dto.SubmissionId,
            description: dto.Description);

        await _documentManager.AttachFileAsync(document, dto.File);
        await _documentManager.SaveDocumentAsync(document);

        var createdDocument = await _unitOfWork.Documents.GetDocumentAsync(document.Id)
            ?? throw new NotFoundException($"Document with id {document.Id} does not exist after creation.");

        return _mapper.Map<DocumentDto>(createdDocument);
    }

    private async Task ValidateCreateAccessAsync(string userId, CreateDocumentDto dto)
    {
        if (dto.SubmissionId is not null)
        {
            var submission = await _unitOfWork.Submissions.GetSubmissionWithRelationsAsync(dto.SubmissionId.Value)
                ?? throw new NotFoundException($"Submission with id {dto.SubmissionId.Value} was not found.");

            if (submission.StudentId == userId)
            {
                return; // owner allowed direkt
            }

            var course = _lmsRelationResolver.ResolveCourse(submission);
            //await _lmsAccessService.EnsureTeacherForCourseAsync(userId, course);
            return;
        }

        // all other cases
        var targetCourse = await _lmsRelationResolver.ResolveCourseForDocumentTargetAsync(
            dto.CourseId,
            dto.ModuleId,
            dto.ActivityId,
            dto.SubmissionId);

        //await _lmsAccessService.EnsureTeacherForCourseAsync(userId, targetCourse);

    }

    /// <summary>
    /// Updates an existing document.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="dto">The updated document data.</param>
    /// <exception cref="NotFoundException">
    /// Thrown if the document does not exist.
    /// </exception>
    public async Task<DocumentDto> UpdateDocumentAsync(int id, string userId, UpdateDocumentDto dto)
    {
        var document = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(id, trackChanges: true) ??
            throw new NotFoundException($"Document with id {id} does not exist");

        await EnsureDocumentExistsAndModifiableAsync(userId, document);

        document.DisplayName = dto.DisplayName ?? string.Empty;
        document.Description = dto.Description ?? string.Empty;
        document.CourseId = dto.CourseId.HasValue ? dto.CourseId.Value : null;
        document.ModuleId = dto.ModuleId.HasValue ? dto.ModuleId.Value : null;
        document.ActivityId = dto.ActivityId.HasValue ? dto.ActivityId.Value : null;
        document.SubmissionId = dto.SubmissionId.HasValue ? dto.SubmissionId.Value : null;

        _unitOfWork.Documents.Update(document);
        await _unitOfWork.CompleteAsync();
        var updatedDocument = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(document.Id);
        return _mapper.Map<DocumentDto>(updatedDocument);
    }

    /// <summary>
    /// Deletes a document if the user has access.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="userId">The user requesting deletion.</param>
    /// <exception cref="NotFoundException">
    /// Thrown if the document does not exist or the user is not authorized.
    /// </exception>
    public async Task DeleteDocumentAsync(int id, string userId)
    {
        var document = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(id)
            ?? throw new NotFoundException($"Document with id: '{id}' does not exist");

        await EnsureDocumentExistsAndDeletableAsync(userId, document);

        // TODO: option to soft delete instead of hard delete, add IsDeleted property to Document entity and filter it out in queries

        // remove the file from storage if it exists
        await _documentManager.DeleteFileAsync(document);

        _unitOfWork.Documents.Delete(document);
        await _unitOfWork.CompleteAsync();
    }

    private async Task EnsureDocumentExistsAndAccessibleAsync(string userId, Document document, CancellationToken ct = default)
    {
        try
        {
            await _lmsAccessService.EnsureCanAccessDocumentAsync(userId, document, ct);
        }
        catch (ForbiddenException)
        {
            // don't reveal the existence of the document if the user is not authorized to access it
            throw new NotFoundException($"Document with id {document.Id} does not exist");
        }
    }

    private async Task EnsureDocumentExistsAndModifiableAsync(string userId, Document document, CancellationToken ct = default)
    {
        try
        {
            await _lmsAccessService.EnsureCanModifyDocumentAsync(userId, document, ct);
        }
        catch (ForbiddenException)
        {
            // don't reveal the existence of the document if the user is not authorized to access it
            throw new NotFoundException($"Document with id {document.Id} does not exist");
        }
    }
    private async Task EnsureDocumentExistsAndDeletableAsync(string userId, Document document, CancellationToken ct = default)
    {
        try
        {
            await _lmsAccessService.EnsureCanDeleteDocumentAsync(userId, document, ct);
        }
        catch (ForbiddenException)
        {
            // don't reveal the existence of the document if the user is not authorized to access it
            throw new NotFoundException($"Document with id {document.Id} does not exist");
        }
    }
}