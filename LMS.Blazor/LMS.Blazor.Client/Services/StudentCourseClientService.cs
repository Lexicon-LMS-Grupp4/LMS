using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;

namespace LMS.Blazor.Client.Services;

public class StudentCourseClientService : IStudentCourseClientService
{
    private const string MyCourseEndpoint = "api/studentcourse/mycourse";
    private const string ClassmatesEndpoint = "api/studentcourse/classmates";

    private readonly IApiService _apiService;

    public StudentCourseClientService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<CourseDto?> GetMyCourseAsync(CancellationToken ct = default) =>
        _apiService.GetAllowNotFoundAsync<CourseDto>(MyCourseEndpoint, ct);

    public async Task<IReadOnlyList<StudentDto>> GetClassmatesAsync(CancellationToken ct = default)
    {
        var list = await _apiService.GetAllowNotFoundAsync<List<StudentDto>>(ClassmatesEndpoint, ct);
        return list ?? new List<StudentDto>();
    }
}
