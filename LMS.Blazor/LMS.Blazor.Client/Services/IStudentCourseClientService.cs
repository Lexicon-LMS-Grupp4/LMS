using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;

namespace LMS.Blazor.Client.Services;

public interface IStudentCourseClientService
{
    Task<CourseDto?> GetMyCourseAsync(CancellationToken ct = default);

    Task<IReadOnlyList<StudentDto>> GetClassmatesAsync(CancellationToken ct = default);
}
