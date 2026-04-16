using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;

namespace LMS.Blazor.Client.Services;

public class CourseService(IApiService apiService) : ICourseService
{
    private readonly IApiService _apiService = apiService;

    private const string Base = "api/courses";
    private const string StudentCourseBase = "api/studentcourse";

    public async Task<PagedResultDto<CourseDto>?> GetAllCoursesAsync(int page = 1, int pageSize = 10, CancellationToken ct = default)
        => await _apiService.GetAsync<PagedResultDto<CourseDto>>($"{Base}?page={page}&pageSize={pageSize}", ct);
    public async Task<CourseDto?> GetCourseByIdAsync(int courseId, CancellationToken ct = default)
        => await _apiService.GetAsync<CourseDto>($"{Base}/{courseId}", ct);
    public async Task<CourseDetailDto?> GetCourseDetailByIdAsync(int courseId, CancellationToken ct = default)
        => await _apiService.GetAsync<CourseDetailDto>($"{Base}/{courseId}/detail", ct);
    public async Task<CourseDto?> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default)
        => await _apiService.PostAsync<CourseDto>(Base, dto, ct);
    public async Task UpdateCourseAsync(int courseId, UpdateCourseDto dto, CancellationToken ct = default)
        => await _apiService.PutAsync<object>($"{Base}/{courseId}", dto, ct);
    public async Task DeleteCourseAsync(int courseId, CancellationToken ct = default)
        => await _apiService.DeleteAsync($"{Base}/{courseId}", ct);
    public async Task<CourseDto?> GetMyCourseAsync(CancellationToken ct = default)
        => await _apiService.GetAllowNotFoundAsync<CourseDto>($"{StudentCourseBase}/mycourse", ct);
}
