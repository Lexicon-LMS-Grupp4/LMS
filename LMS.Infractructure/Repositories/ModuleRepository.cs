using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ModuleRepository(ApplicationDbContext context) : RepositoryBase<Module>(context), IModuleRepository
{
    public IQueryable<Module> BuildQuery(int? courseId = null, bool trackChanges = false)
    {
        return FindByCondition(
                m => courseId == null || m.CourseId == courseId.Value,
                trackChanges)
            .Include(m => m.Course)
            .Include(m => m.Activities);
    }
    public async Task<Module?> GetModuleAsync(int id, bool trackChanges = false)
    {
        return await FindByCondition(m => m.Id == id, trackChanges)
            .Include(m => m.Activities)
                .ThenInclude(a => a.ActivityType)
            .Include(m => m.Activities)
                .ThenInclude(a => a.Submissions)
            .Include(m => m.Activities)
                .ThenInclude(a => a.Documents)
            .Include(m => m.Documents)
                .ThenInclude(d => d.UploadedByUser)
            .Include(m => m.Course)
            .FirstOrDefaultAsync();
    }

    public async Task<Module?> GetModuleByNameAsync(int courseId, string name, bool trackChanges = false)
    {
        var query = FindByCondition(
           m => (m.CourseId == courseId && m.Name == name),
           trackChanges);

        return await query.FirstOrDefaultAsync();
    }
}