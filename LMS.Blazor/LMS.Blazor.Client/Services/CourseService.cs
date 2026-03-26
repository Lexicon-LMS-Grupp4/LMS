using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class CourseService : ICourseService
{
    private readonly IApiService _apiService;
    private readonly ILogger<CourseService> _logger;

    private const string Base = "api/courses";

    public CourseService(IApiService apiService, ILogger<CourseService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResultDto<CourseDto>?> GetAllCoursesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<PagedResultDto<CourseDto>>($"{Base}?page={page}&pageSize={pageSize}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching all courses.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize courses response.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out fetching all courses.");
        }
        return null;
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<CourseDto>($"{Base}/{courseId}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching course {CourseId}.", courseId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize course {CourseId}.", courseId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out fetching course {CourseId}.", courseId);
        }
        return null;
    }

    public async Task<CourseDetailDto?> GetCourseDetailByIdAsync(int courseId, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<CourseDetailDto>($"{Base}/{courseId}/detail", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching course detail {CourseId}.", courseId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize course detail {CourseId}.", courseId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out fetching course detail {CourseId}.", courseId);
        }
        return null;
    }

    public async Task<CourseDto?> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<CourseDto>(Base, dto, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error creating course.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize create course response.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out creating course.");
        }
        return null;
    }

    public async Task UpdateCourseAsync(int courseId, UpdateCourseDto dto, CancellationToken ct = default)
    {
        try
        {
            await _apiService.PutAsync<object>($"{Base}/{courseId}", dto, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error updating course {CourseId}.", courseId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize update course {CourseId} response.", courseId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out updating course {CourseId}.", courseId);
        }
    }

    public async Task DeleteCourseAsync(int courseId, CancellationToken ct = default)
    {
        try
        {
            await _apiService.DeleteAsync($"{Base}/{courseId}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error deleting course {CourseId}.", courseId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timed out deleting course {CourseId}.", courseId);
        }
    }
}