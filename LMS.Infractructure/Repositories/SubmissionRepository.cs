using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class SubmissionRepository(ApplicationDbContext context) : RepositoryBase<Submission>(context), ISubmissionRepository
{
    public async Task<(IEnumerable<Submission> submissions, int totalCount)> GetAllSubmissionsAsync(int page,
                                                                                                    int pageSize,
                                                                                                    int? activityId,
                                                                                                    string? studentId,
                                                                                                    bool trackChanges = false)
    {
        var query = FindByCondition(
            s => (activityId == null || s.ActivityId == activityId.Value)
                    && (string.IsNullOrEmpty(studentId) || s.StudentId == studentId),
            trackChanges)
            .Include(s => s.Student)
            .Include(s => s.Activity)
            .Include(s => s.Comments)
            .Include(s => s.Document);

        return await query.PagedResult(page, pageSize);
    }

    public IQueryable<Submission> BuildQuery(int? activityId = null, string? studentId = null, bool trackChanges = false)
    {
        return FindByCondition(
                s =>
                    (activityId == null || s.ActivityId == activityId.Value) &&
                    (studentId == null || s.StudentId == studentId),
                trackChanges)
            .Include(a => a.Activity).ThenInclude(a => a.Module).ThenInclude(m => m.Course)
            .Include(a => a.Student)
            .Include(a => a.Document);
    }

    public async Task<Submission?> GetByActivityAndStudentAsync(int activityId, string studentId, bool trackChanges = false)
    {
        return await FindByCondition(
                s => s.ActivityId == activityId && s.StudentId == studentId,
                trackChanges)
            .Include(s => s.Student)
            .Include(s => s.Activity)
                .ThenInclude(a => a.Module)
                    .ThenInclude(m => m.Course)
            .Include(s => s.Document)
            .Include(s => s.Comments)
                .ThenInclude(c => c.Author)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<Submission?> GetSubmissionAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(m => m.Id == id, trackChanges)
            .Include(s => s.Student)
            .Include(s => s.Activity)
            .Include(s => s.Document)
            .Include(s => s.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync();
    }

    public async Task<Submission?> GetSubmissionWithRelationsAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(m => m.Id == id, trackChanges)
            .Include(s => s.Student)
            .Include(s => s.Activity)
                .ThenInclude(a => a.Module)
                    .ThenInclude(m => m.Course)
            .Include(s => s.Document)
            .Include(s => s.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync();
    }
}
