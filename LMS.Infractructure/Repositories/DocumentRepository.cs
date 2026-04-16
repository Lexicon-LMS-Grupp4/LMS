using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using LMS.Shared.DTOs.Document;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LMS.Infractructure.Repositories;

public class DocumentRepository(ApplicationDbContext context) : RepositoryBase<Document>(context), IDocumentRepository
{
    /// <summary>
    /// Retrieves a paged list of documents matching the specified filter.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="dto">Filter criteria.</param>
    /// <returns>A paged result of documents.</returns>
    public async Task<(IEnumerable<Document> documents, int totalCount)> GetDocumentsAsync(int page,
                                                                                            int pageSize,
                                                                                            DocumentQueryDto dto)
    {
        var query = BuildBasicQuery(dto);

        var (documents, totalCount) = await query
            .OrderByDescending(d => d.UploadedAt)
            .PagedResult(page, pageSize);

        return (documents, totalCount);
    }

    /// <summary>
    /// Retrieves a document with access-related data.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>The document if found.</returns>
    public async Task<Document?> GetDocumentWithAccessRelationsAsync(int id, bool trackChanges = false)
    {
        return await BuildAccessRelationsQuery(trackChanges).FirstOrDefaultAsync(d => d.Id == id);
    }
    /// <summary>
    /// Retrieves a document with basic document relations.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>The document if found.</returns>
    public async Task<Document?> GetDocumentAsync(int id, bool trackChanges = false)
    {
        return await BuildBasicQuery(trackChanges).FirstOrDefaultAsync(d => d.Id == id);
    }

    /// <summary>
    /// Builds a query with basic document relations.
    /// </summary>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>A query including basic relations.</returns>
    public IQueryable<Document> BuildBasicQuery(bool trackChanges = false)
    {
        return FindAll(trackChanges)
                .Include(d => d.UploadedByUser)
                .Include(d => d.Course)
                .Include(d => d.Module).ThenInclude(m => m.Course)
                .Include(d => d.Activity).ThenInclude(a => a.Module).ThenInclude(m => m.Course)
                .Include(d => d.Submission).ThenInclude(s => s.Activity).ThenInclude(a => a.Module).ThenInclude(m => m.Course);
    }

    /// <summary>
    /// Builds a filtered query with basic document relations.
    /// </summary>
    /// <param name="dto">Filter criteria.</param>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>A filtered query.</returns>
    public IQueryable<Document> BuildBasicQuery(DocumentQueryDto dto, bool trackChanges = false)
    {
        return ApplyDocumentFilters(BuildBasicQuery(trackChanges), dto);
    }

    /// <summary>
    /// Builds a query with relations required for access checks.
    /// </summary>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>A query including access-related relations.</returns>
    public IQueryable<Document> BuildAccessRelationsQuery(bool trackChanges = false)
    {
        return FindAll(trackChanges)
                .Include(d => d.UploadedByUser)
                .Include(d => d.Course).ThenInclude(c => c.CourseTeachers)
                .Include(d => d.Module).ThenInclude(m => m.Course).ThenInclude(c => c.CourseTeachers)
                .Include(d => d.Activity).ThenInclude(a => a.Module).ThenInclude(m => m.Course).ThenInclude(c => c.CourseTeachers)
                .Include(d => d.Submission).ThenInclude(s => s.Activity).ThenInclude(a => a.Module).ThenInclude(m => m.Course).ThenInclude(c => c.CourseTeachers);
    }

    /// <summary>
    /// Builds a filtered query with relations required for access checks.
    /// </summary>
    /// <param name="dto">Filter criteria.</param>
    /// <param name="trackChanges">Enables change tracking if true.</param>
    /// <returns>A filtered query.</returns>
    public IQueryable<Document> BuildAccessRelationsQuery(DocumentQueryDto dto, bool trackChanges = false)
    {
        return ApplyDocumentFilters(BuildAccessRelationsQuery(trackChanges), dto);
    }

    /// <summary>
    /// Applies document filters to a query.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="dto">Filter criteria.</param>
    /// <returns>The filtered query.</returns>
    private static IQueryable<Document> ApplyDocumentFilters(IQueryable<Document> query, DocumentQueryDto dto)
    {
        var q = query;

        // Apply Layer filtering
        q = q.WhereIf(!string.IsNullOrEmpty(dto.ScopeTarget) && dto.ScopeTarget == "Course", d => d.CourseId != null && d.ModuleId == null && d.ActivityId == null && d.SubmissionId == null)
             .WhereIf(!string.IsNullOrEmpty(dto.ScopeTarget) && dto.ScopeTarget == "Module", d => d.ModuleId != null && d.ActivityId == null && d.SubmissionId == null)
             .WhereIf(!string.IsNullOrEmpty(dto.ScopeTarget) && dto.ScopeTarget == "Activity", d => d.ActivityId != null && d.SubmissionId == null);

        // Apply Parent filtering
        if (dto.CourseId.HasValue && dto.CourseId.Value > 0)
        {
            q = q.Where(d =>
                d.CourseId == dto.CourseId.Value ||
                (d.Module != null && d.Module.CourseId == dto.CourseId.Value) ||
                (d.Activity != null && d.Activity.Module != null && d.Activity.Module.CourseId == dto.CourseId.Value));
        }

        if (dto.ModuleId.HasValue && dto.ModuleId.Value > 0)
        {
            q = q.Where(d =>
                d.ModuleId == dto.ModuleId.Value ||
                (d.Activity != null && d.Activity.ModuleId == dto.ModuleId.Value));
        }

        if (dto.ActivityId.HasValue && dto.ActivityId.Value > 0)
        {
            q = q.Where(d => d.ActivityId == dto.ActivityId.Value);
        }

        if (dto.SubmissionId.HasValue && dto.SubmissionId.Value > 0)
        {
            q = q.Where(d => d.SubmissionId == dto.SubmissionId.Value);
        }

        return q;
    }

}