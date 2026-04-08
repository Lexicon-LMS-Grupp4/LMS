using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Document;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services;
/// <summary>
/// Provides operations for managing documents, including retrieval, creation, update, deletion,
/// and access control based on user roles and course relationships.
/// </summary>
public class DocumentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileStorage fileStorage,
    IUserService userService,
    UserManager<ApplicationUser> userManager) : IDocumentService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IFileStorage _fileStorage = fileStorage;

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
        // TODO: don't use UserService in this service, instead create UserRepository for
        // getting GetUserWithRelationsAsync
        var user = await userService.GetUserWithRelationsAsync(userId) ??
            throw new UnauthorizedAccessException($"User by id {userId} does not exist");

        var isTeacher = await _userManager.IsInRoleAsync(user, "Teacher");

        var query = ApplyDocumentAccessFilter(
            _unitOfWork.Documents.BuildBasicQuery(dto),
            userId,
            isTeacher,
            isTeacher ? user.TeachingCourses.Select(tc => tc.CourseId).ToList() :
                new List<int> { user.CourseId ?? 0 });


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

        await EnsureDocumentAccess(userId, document);

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

        await EnsureDocumentAccess(userId, document);

        if (string.IsNullOrWhiteSpace(document.StoredFileName))
        {
            throw new NotFoundException("File not found.");
        }

        var stream = await _fileStorage.OpenReadAsync(document.StoredFileName);

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

        var user = await _userManager.FindByIdAsync(userId) ??
            throw new UnauthorizedAccessException($"User by id {userId} does not exist");

        await ValidateCreateAccessAsync(userId, dto);

        var savedFileResult = await _fileStorage.SaveAsync(dto.File);

        var document = _mapper.Map<Document>(dto);

        document.UploadedByUser = user;
        document.UploadedAt = DateTime.UtcNow;
        document.FileSize = savedFileResult.FileSize;
        document.StoredFileName = savedFileResult.FileName;

        var course = ResolveCourse(document);

        _unitOfWork.Documents.Create(document);
        await _unitOfWork.CompleteAsync();

        var createdDocument = await _unitOfWork.Documents.GetDocumentWithAccessRelationsAsync(document.Id);

        return _mapper.Map<DocumentDto>(createdDocument);
    }

    private async Task ValidateCreateAccessAsync(string userId, CreateDocumentDto dto)
    {
        if (dto.SubmissionId is not null)
        {
            // TODO: avoid this duplicated logic with EnsureDocumentAccess, refactor it and make it more generic, maybe move it to its own service
            var submission = await _unitOfWork.Submissions.GetSubmissionWithRelationsAsync(dto.SubmissionId.Value)
                ?? throw new NotFoundException($"Submission with id {dto.SubmissionId.Value} was not found.");

            if (submission.StudentId == userId)
            {
                return; // owner allowed direkt
            }

            EnsureTeacherForCourse(userId, submission.Activity.Module.Course);
            return;
        }

        // alla andra fall
        var course = await ResolveCourseForDocumentAsync(dto);
        EnsureTeacherForCourse(userId, course);

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
        var document = await _unitOfWork.Documents.GetDocumentAsync(id, trackChanges: true) ??
            throw new NotFoundException($"Document with id {id} does not exist");

        await EnsureDocumentAccess(userId, document);

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

        await EnsureDocumentAccess(userId, document);

        // TODO: option to soft delete instead of hard delete, add IsDeleted property to Document entity and filter it out in queries

        // remove the file from storage if it exists
        if (!string.IsNullOrWhiteSpace(document.StoredFileName))
        {
            var success = await _fileStorage.DeleteAsync(document.StoredFileName);
            if (!success)
            {
                // log the failure to delete the file, but do not prevent the document record from being deleted
                // consider implementing a retry mechanism or marking the document for cleanup if file deletion fails
            }
        }

        _unitOfWork.Documents.Delete(document);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Ensures that the user has access to the specified document.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="document">The document to check.</param>
    /// <exception cref="NotFoundException">
    /// Thrown if the user is not authorized to access the document.
    /// </exception>
    private async Task EnsureDocumentAccess(string userId, Document document)
    {
        if (document.UploadedByUserId != userId)
        {
            var course = ResolveCourse(document);

            if (course == null || !IsTeacherForCourse(userId, course))
            {
                // pretend the file doesn't exist to prevent data leakage
                throw new NotFoundException($"Document with id {document.Id} does not exist");
            }
        }
    }

    /// <summary>
    /// Resolves the course associated with a document.
    /// </summary>
    /// <param name="d">The document.</param>
    /// <returns>The associated course, if any.</returns>
    private static Course? ResolveCourse(Document d)
    {
        return d.Course
            ?? d.Module?.Course
            ?? d.Activity?.Module?.Course
            ?? d.Submission?.Activity?.Module?.Course;
    }

    private async Task<Course> ResolveCourseForDocumentAsync(CreateDocumentDto dto)
    {
        // TODO: refactor this and ResolveCourse, make it more generic and take e special dto with only
        // ActivityId, ModuleId, CourseId and SubmissionId, and move it to its own service
        if (dto.CourseId is not null)
        {
            return await _unitOfWork.Courses.GetCourseAsync(dto.CourseId.Value)
                ?? throw new NotFoundException($"Course with id {dto.CourseId.Value} was not found.");
        }

        if (dto.ModuleId is not null)
        {
            var module = await _unitOfWork.Modules.GetModuleAsync(dto.ModuleId.Value)
                ?? throw new NotFoundException($"Module with id {dto.ModuleId.Value} was not found.");

            return module.Course;
        }

        if (dto.ActivityId is not null)
        {
            var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(dto.ActivityId.Value)
                ?? throw new NotFoundException($"Activity with id {dto.ActivityId.Value} was not found.");

            return activity.Module.Course;
        }

        if (dto.SubmissionId is not null)
        {
            var submission = await _unitOfWork.Submissions.GetSubmissionWithRelationsAsync(dto.SubmissionId.Value)
                ?? throw new NotFoundException($"Submission with id {dto.SubmissionId.Value} was not found.");

            return submission.Activity.Module.Course;
        }

        throw new BadRequestException("Document must belong to a course, module, activity or submission.");
    }

    /// <summary>
    /// Determines whether a user is a teacher for a given course.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="course">The course.</param>
    /// <returns><c>true</c> if the user is a teacher for the course; otherwise, <c>false</c>.</returns>
    private static bool IsTeacherForCourse(string userId, Course course)
    {
        return course.CourseTeachers.Any(t => t.TeacherId == userId);
    }

    private static void EnsureTeacherForCourse(string userId, Course course)
    {
        if (!IsTeacherForCourse(userId, course))
        {
            throw new ForbiddenException("Only teachers for this course allowed for this operation.");
        }
    }

    /// <summary>
    /// Applies access control rules to a document query.
    /// </summary>
    /// <param name="query">The document query.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="isTeacher">Indicates whether the user is a teacher.</param>
    /// <param name="allowedCourseIds">The course IDs the user has access to.</param>
    /// <returns>A filtered query containing only accessible documents.</returns>
    private IQueryable<Document> ApplyDocumentAccessFilter(IQueryable<Document> query,
                                                           string userId,
                                                           bool isTeacher,
                                                           List<int> allowedCourseIds)
    {
        if (isTeacher)
        {
            return query.Where(d =>
                (d.SubmissionId == null && (
                    (d.CourseId != null && allowedCourseIds.Contains(d.CourseId.Value)) ||
                    (d.Module != null && allowedCourseIds.Contains(d.Module.CourseId)) ||
                    (d.Activity != null && allowedCourseIds.Contains(d.Activity.Module.CourseId))
                ))
                ||
                (d.Submission != null && allowedCourseIds.Contains(d.Submission.Activity.Module.CourseId))
            );
        }

        return query.Where(d =>
            (d.SubmissionId == null && (
                (d.CourseId != null && allowedCourseIds.Contains(d.CourseId.Value)) ||
                (d.Module != null && allowedCourseIds.Contains(d.Module.CourseId)) ||
                (d.Activity != null && allowedCourseIds.Contains(d.Activity.Module.CourseId))
            ))
            ||
            (d.Submission != null && d.Submission.StudentId == userId)
        );
    }
}