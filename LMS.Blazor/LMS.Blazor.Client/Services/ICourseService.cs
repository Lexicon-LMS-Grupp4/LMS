using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;

namespace LMS.Blazor.Client.Services;

public interface ICourseService
{
    Task<PagedResultDto<CourseDto>?> GetAllCoursesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
    /// <summary>Current student's enrolled course (404 → null).</summary>
    Task<CourseDto?> GetMyCourseAsync(CancellationToken ct = default);
    Task<CourseDto?> GetCourseByIdAsync(int courseId, CancellationToken ct = default);
    Task<CourseDetailDto?> GetCourseDetailByIdAsync(int courseId, CancellationToken ct = default);
    Task<CourseDto?> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default);
    Task UpdateCourseAsync(int courseId, UpdateCourseDto dto, CancellationToken ct = default);
    Task DeleteCourseAsync(int courseId, CancellationToken ct = default);
}