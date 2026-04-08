using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.User;

namespace LMS.Blazor.Client.Services;

public interface IUserService
{
    Task<PagedResultDto<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 100, CancellationToken ct = default);
    Task<PagedResultDto<UserDto>?> GetUsersWithoutCourseAsync(int page = 1, int pageSize = 100, CancellationToken ct = default);
    Task<List<StudentDto>?> GetUsersByCourseAsync(int courseId, CancellationToken ct = default);
    Task<PagedResultDto<UserDto>?> GetTeachersAsync(int page = 1, int pageSize = 100, CancellationToken ct = default);
    Task<bool> EnrollUserInCourseAsync(string userId, int courseId, CancellationToken ct = default);
    Task<bool> RemoveUserFromCourseAsync(string userId, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(string userId, CancellationToken ct = default);
}