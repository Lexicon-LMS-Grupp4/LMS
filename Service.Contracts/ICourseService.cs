using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;

namespace Service.Contracts;

/// <summary>
/// Service contract for Course operations
/// </summary>
public interface ICourseService
{
    Task<PagedResultDto<CourseDto>> GetAllCoursesAsync(string userId, int page, int pageSize);
    Task<CourseDto> GetCourseByIdAsync(int id, string userId);
    Task<CourseDetailDto> GetCourseDetailByIdAsync(int id, string userId);
    Task<CourseDto> CreateCourseAsync(string userId, CreateCourseDto dto);
    Task UpdateCourseAsync(int id, string userId, UpdateCourseDto dto);
    Task DeleteCourseAsync(int id, string userId);
}