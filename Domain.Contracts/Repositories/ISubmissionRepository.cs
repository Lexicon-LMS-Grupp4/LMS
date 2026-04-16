using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ISubmissionRepository : IRepositoryBase<Submission>
{
    IQueryable<Submission> BuildQuery(int? activityId = null, string? studentId = null, bool trackChanges = false);
    Task<(IEnumerable<Submission> submissions, int totalCount)> GetAllSubmissionsAsync(
        int page, int pageSize, int? activityId, string? studentId, bool trackChanges = false);
    Task<Submission?> GetByActivityAndStudentAsync(int activityId, string studentId, bool trackChanges = false);
    Task<Submission?> GetSubmissionAsync(int id, bool trackChanges = false);
    Task<Submission?> GetSubmissionWithRelationsAsync(int id, bool trackChanges = false);
}
