using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>
{
    Task<Activity?> GetActivityWithDetailAsync(int? id);
    Task<(IEnumerable<Activity> activities, int totalCount)> GetAllActivitiesAsync(
        int page, int pageSize, int? moduleId, bool trackChanges = false);
    Task<Activity?> GetActivityWithRelationsAsync(int id, bool trackChanges = false);
}