using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ISubmissionRepository : IRepositoryBase<Submission>
{
    Task<(IEnumerable<Submission> submissions, int totalCount)> GetAllSubmissionsAsync(
        int page, int pageSize, int? activityId, string? studentId, bool trackChanges = false);
    Task<Submission?> GetSubmissionAsync(int id, bool trackChanges = false);
    Task<Submission?> GetSubmissionWithRelationsAsync(int id, bool trackChanges = false);
}