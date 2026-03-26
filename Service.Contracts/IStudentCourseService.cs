using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;

namespace Service.Contracts;

public interface IStudentCourseService
{
    Task<CourseDto?> GetStudentCourseAsync(string userId);

    Task<IReadOnlyList<StudentDto>> GetCourseClassmatesAsync(int courseId, string excludeUserId);
}
