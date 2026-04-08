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
            .Include(a => a.Submissions)
            .FirstAsync(a => a.Id == id);
    }

    public async Task<Activity?> GetActivityWithRelationsAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(a => a.Id == id, trackChanges)
            .Include(a => a.Module)
                .ThenInclude(m => m.Course)
            .Include(a => a.Documents)
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Activity> activities, int totalCount)> GetAllActivitiesAsync(
        int page, int pageSize, int? moduleId, bool trackChanges = false)
    {
        var query = FindByCondition(a => moduleId == null || a.ModuleId == moduleId.Value, trackChanges);

        return await query.PagedResult(page, pageSize);
    }

}