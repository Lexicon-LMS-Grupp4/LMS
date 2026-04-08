using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;

namespace Service.Contracts;

/// <summary>
/// Service contract for Course operations
/// </summary>
public interface ICourseService
{
    Task<PagedResultDto<CourseDto>> GetAllCoursesAsync(int page, int pageSize);
    Task<CourseDto> GetCourseByIdAsync(int id);
    Task<CourseDetailDto> GetCourseDetailByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task UpdateCourseAsync(int id, UpdateCourseDto dto);
    Task DeleteCourseAsync(int id);
}