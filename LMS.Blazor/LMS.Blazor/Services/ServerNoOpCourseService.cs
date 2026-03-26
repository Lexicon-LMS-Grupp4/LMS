using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;

namespace LMS.Blazor.Services;

public class ServerNoOpCourseService(ILogger<ServerNoOpCourseService> logger) : ICourseService
{
    private readonly ILogger<ServerNoOpCourseService> _logger = logger;

    public Task<PagedResultDto<CourseDto>?> GetAllCoursesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for GetAllCoursesAsync");
        return Task.FromResult<PagedResultDto<CourseDto>?>(null);
    }

    public Task<CourseDto?> GetCourseByIdAsync(int courseId, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for GetCourseByIdAsync: {CourseId}", courseId);
        return Task.FromResult<CourseDto?>(null);
    }

    public Task<CourseDetailDto?> GetCourseDetailByIdAsync(int courseId, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for GetCourseDetailByIdAsync: {CourseId}", courseId);
        return Task.FromResult<CourseDetailDto?>(null);
    }

    public Task<CourseDto?> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for CreateCourseAsync");
        return Task.FromResult<CourseDto?>(null);
    }

    public Task UpdateCourseAsync(int courseId, UpdateCourseDto dto, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for UpdateCourseAsync: {CourseId}", courseId);
        return Task.CompletedTask;
    }

    public Task DeleteCourseAsync(int courseId, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpCourseService called for DeleteCourseAsync: {CourseId}", courseId);
        return Task.CompletedTask;
    }
}