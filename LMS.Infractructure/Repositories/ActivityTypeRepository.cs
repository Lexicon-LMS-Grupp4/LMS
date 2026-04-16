using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityTypeRepository(ApplicationDbContext context)
    : RepositoryBase<ActivityType>(context), IActivityTypeRepository
{
    public async Task<IEnumerable<ActivityType>> GetAllActivityTypesAsync(bool trackChanges = false)
        => await FindAll(trackChanges).OrderBy(at => at.Name).ToListAsync();

    public async Task<ActivityType?> GetActivityTypeByNameAsync(string name, bool trackChanges = false)
        => await FindByCondition(at => at.Name == name, trackChanges).FirstOrDefaultAsync();
}