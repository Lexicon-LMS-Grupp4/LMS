using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetAllCoursesAsync(
        int page, int pageSize, bool trackChanges = false)
    {
        var query = BuildQuery(trackChanges);

        return await query.PagedResult(page, pageSize);
    }

    public IQueryable<Course> BuildQuery(bool trackChanges = false)
    {
        return FindAll(trackChanges)
            .Include(c => c.Students)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Activities)
            .Include(c => c.CourseTeachers)
            .Include(c => c.Documents)
                .ThenInclude(d => d.UploadedByUser);
    }

    public async Task<Course?> GetCourseAsync(int id, bool trackChanges = false)
    {
        return await BuildQuery(trackChanges)
            .FirstOrDefaultAsync(c => c.Id == id);

    }
}