using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>
{
    IQueryable<Activity> BuildQuery(int? moduleId = null, bool trackChanges = false);
    Task<Activity?> GetActivityWithDetailAsync(int? id);
    Task<Activity?> GetActivityWithRelationsAsync(int id, bool trackChanges = false);
    Task<bool> AnyWithActivityTypeAsync(int activityTypeId);
}