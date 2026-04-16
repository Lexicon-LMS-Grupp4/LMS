using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository(ApplicationDbContext context) : RepositoryBase<Activity>(context), IActivityRepository
{
    public async Task<Activity?> GetActivityWithDetailAsync(int? id)
    {
        return await DbSet
            .Include(a => a.Documents)
                .ThenInclude(d => d.UploadedByUser)
            .Include(a => a.Submissions)
                .ThenInclude(s => s.Document)
            .FirstAsync(a => a.Id == id);
    }

    public async Task<Activity?> GetActivityWithRelationsAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(a => a.Id == id, trackChanges)
            .Include(a => a.Module)
            .ThenInclude(m => m.Course)
            .Include(a => a.ActivityType)
            .Include(a => a.Documents)
                .ThenInclude(d => d.UploadedByUser)
            .Include(a => a.Submissions)
                .ThenInclude(s => s.Document)
            .Include(a => a.Submissions)
                .ThenInclude(s => s.Student)
            .Include(a => a.Submissions)
                .ThenInclude(s => s.Comments)
                    .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync();
    }

    public IQueryable<Activity> BuildQuery(int? moduleId = null, bool trackChanges = false)
    {
        return FindByCondition(
                a => moduleId == null || a.ModuleId == moduleId.Value,
                trackChanges)
            .Include(a => a.Submissions)
            .Include(a => a.Module).ThenInclude(m => m.Course);
    }

    public async Task<(IEnumerable<Activity> activities, int totalCount)> GetAllActivitiesAsync(
       int page, int pageSize, int? moduleId, bool trackChanges = false)
    {
        var query = FindByCondition(a => moduleId == null || a.ModuleId == moduleId.Value, trackChanges);

        return await query.PagedResult(page, pageSize);
    }

    public async Task<bool> AnyWithActivityTypeAsync(int activityTypeId)
        => await FindByCondition(a => a.ActivityTypeId == activityTypeId).AnyAsync();

}
