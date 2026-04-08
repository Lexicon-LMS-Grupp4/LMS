using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.User;

namespace LMS.Blazor.Client.Services;

public class UserService(IApiService apiService) : IUserService
{
    private readonly IApiService _apiService = apiService;
    private const string Base = "api/users";

    public Task<PagedResultDto<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 100, CancellationToken ct = default)
        => _apiService.GetAsync<PagedResultDto<UserDto>>($"{Base}?page={page}&pageSize={pageSize}", ct);

    public Task<PagedResultDto<UserDto>?> GetUsersWithoutCourseAsync(int page = 1, int pageSize = 100, CancellationToken ct = default)
        => _apiService.GetAsync<PagedResultDto<UserDto>>($"{Base}/without-course?page={page}&pageSize={pageSize}", ct);

    public Task<List<StudentDto>?> GetUsersByCourseAsync(int courseId, CancellationToken ct = default)
        => _apiService.GetAsync<List<StudentDto>>($"{Base}/course/{courseId}", ct);

    public Task<PagedResultDto<UserDto>?> GetTeachersAsync(int page = 1, int pageSize = 100, CancellationToken ct = default)
        => _apiService.GetAsync<PagedResultDto<UserDto>>($"{Base}/teachers?page={page}&pageSize={pageSize}", ct);

    public async Task<bool> EnrollUserInCourseAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var result = await _apiService.PutAsync<object>($"{Base}/{userId}/enroll/{courseId}", new { }, ct);
        return result is not null;
    }

    public async Task<bool> RemoveUserFromCourseAsync(string userId, CancellationToken ct = default)
    {
        var result = await _apiService.PutAsync<object>($"{Base}/{userId}/remove-course", new { }, ct);
        return result is not null;
    }

    public Task<bool> DeleteUserAsync(string userId, CancellationToken ct = default)
        => _apiService.DeleteAsync($"{Base}/{userId}", ct);
}