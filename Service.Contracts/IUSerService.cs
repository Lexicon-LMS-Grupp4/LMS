using Domain.Models.Entities;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace Service.Contracts;

public interface IUserService
{
    Task<PagedResultDto<UserDto>> GetAllStudentsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<StudentDto>> GetUsersByCourseAsync(int courseId, CancellationToken ct = default);
    Task<PagedResultDto<UserDto>> GetUsersWithoutCourseAsync(int page, int pageSize, CancellationToken ct = default);
    Task<PagedResultDto<UserDto>> GetTeachersAsync(int page, int pageSize, CancellationToken ct = default);
    Task<(IdentityResult, UserDto?)> CreateUserAsync(CreateUserDto userDto, CancellationToken ct = default);
    Task EnrollUserInCourseAsync(string userId, int courseId, CancellationToken ct = default);
    Task RemoveUserFromCourseAsync(string userId, CancellationToken ct = default);
    Task DeleteUserAsync(string currentUserId, string targetUserId, CancellationToken ct = default);
    Task<UserDto> UpdateUserAsync(string userId, UpdateUserDto dto, CancellationToken ct = default);
    Task<ApplicationUser?> GetUserWithRelationsAsync(string userId, CancellationToken ct = default);
    Task<IEnumerable<UserDto>> GetTeachersByCourseAsync(int courseId, CancellationToken ct = default);
}
