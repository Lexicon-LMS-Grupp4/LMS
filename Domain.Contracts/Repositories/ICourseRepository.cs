using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface ICourseRepository : IRepositoryBase<Course>
{
    Task<(IEnumerable<Course> Courses, int TotalCount)> GetAllCoursesAsync(
        int page, int pageSize, bool trackChanges = false);
    IQueryable<Course> BuildQuery(bool trackChanges = false);
    Task<Course?> GetCourseAsync(int id, bool trackChanges = false);
}