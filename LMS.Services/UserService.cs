using AutoMapper;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _mapper = mapper;
    }

    // helpers
    private async Task<string?> GetRoleIdAsync(string roleName, CancellationToken ct) =>
        await _db.Roles
            .Where(r => r.Name == roleName)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(ct);

    private IQueryable<ApplicationUser> UsersInRole(string roleId) =>
        _db.Users.Where(u =>
            _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));

    private async Task<UserDto> MapToUserDtoAsync(ApplicationUser u)
    {
        var roles = await _userManager.GetRolesAsync(u);
        return new UserDto
        {
            Id = u.Id,
            Name = u.UserName ?? string.Empty,
            Email = u.Email ?? string.Empty,
            Role = roles.FirstOrDefault() ?? "Unknown",
            CourseId = u.CourseId,
            CourseName = u.Course?.Name
        };
    }

    private async Task<PagedResultDto<UserDto>> ToPagedResultAsync(
    IQueryable<ApplicationUser> query, int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = new List<UserDto>(users.Count);
        foreach (var u in users)
            items.Add(await MapToUserDtoAsync(u));

        return new PagedResultDto<UserDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    // queries
    public async Task<PagedResultDto<UserDto>> GetAllStudentsAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var roleId = await GetRoleIdAsync("Student", ct);
        if (roleId is null) return new PagedResultDto<UserDto>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = page,
            PageSize = pageSize
        };

        var query = UsersInRole(roleId)
            .Include(u => u.Course)
            .OrderBy(u => u.Email);

        return await ToPagedResultAsync(query, page, pageSize, ct);
    }

    public async Task<UserDto?> GetUserByIdAsync(string id, CancellationToken ct = default)
    {
        var user = await _db.Users
            .Include(u => u.Course)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        return user is null ? null : await MapToUserDtoAsync(user);
    }

    public async Task<ApplicationUser?> GetUserWithRelationsAsync(string userId, CancellationToken ct = default)
    {
        return await _db.Users
            .Include(u => u.TeachingCourses)
            .Include(u => u.UploadedDocuments)
            .Include(u => u.Course)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<IEnumerable<StudentDto>> GetUsersByCourseAsync(
        int courseId, CancellationToken ct = default)
    {
        return await _db.Users
            .Where(u => u.CourseId == courseId)
            .OrderBy(u => u.UserName)
            .Select(u => new StudentDto
            {
                Id = u.Id,
                Name = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty
            })
            .ToListAsync(ct);
    }

    public async Task<PagedResultDto<UserDto>> GetUsersWithoutCourseAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var roleId = await GetRoleIdAsync("Student", ct);
        if (roleId is null) return new PagedResultDto<UserDto>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = page,
            PageSize = pageSize
        };

        var query = UsersInRole(roleId)
            .Where(u => u.CourseId == null)
            .OrderBy(u => u.Email);

        return await ToPagedResultAsync(query, page, pageSize, ct);
    }

    public async Task<PagedResultDto<UserDto>> GetTeachersAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var roleId = await GetRoleIdAsync("Teacher", ct);
        if (roleId is null) return new PagedResultDto<UserDto>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = page,
            PageSize = pageSize
        };

        var query = UsersInRole(roleId)
            .OrderBy(u => u.Email);

        return await ToPagedResultAsync(query, page, pageSize, ct);
    }

    // commands

    public async Task<(IdentityResult, UserDto?)> CreateUserAsync(CreateUserDto createUserDto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(createUserDto);

        var isRoleValid = !string.IsNullOrWhiteSpace(createUserDto.Role);

        if (isRoleValid)
        {
            var roleExists = await _roleManager.RoleExistsAsync(createUserDto.Role);
            if (!roleExists)
                return (IdentityResult.Failed(new IdentityError { Description = "Role does not exist" }), null);
        }

        var user = _mapper.Map<ApplicationUser>(createUserDto);
        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        if (result.Succeeded && isRoleValid)
            result = await _userManager.AddToRoleAsync(user, createUserDto.Role);

        return (result, _mapper.Map<UserDto>(user));
    }

    public async Task EnrollUserInCourseAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");

        var courseExists = await _db.Courses.AnyAsync(c => c.Id == courseId, ct);
        if (!courseExists)
            throw new KeyNotFoundException($"Course '{courseId}' was not found.");

        user.CourseId = courseId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task RemoveUserFromCourseAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");

        user.CourseId = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task DeleteUserAsync(string currentUserId, string targetUserId, CancellationToken ct = default)
    {
        if (currentUserId == targetUserId)
            throw new InvalidOperationException("You cannot delete your own account.");

        var user = await _userManager.FindByIdAsync(targetUserId)
            ?? throw new KeyNotFoundException($"User '{targetUserId}' was not found.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<UserDto> UpdateUserAsync(string userId, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");

        // Role change
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(dto.Role))
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    throw new InvalidOperationException(
                        string.Join(", ", removeResult.Errors.Select(e => e.Description)));

                var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addResult.Succeeded)
                    throw new InvalidOperationException(
                        string.Join(", ", addResult.Errors.Select(e => e.Description)));
            }
        }

        // Course assignment
        if (dto.CourseId is not null)
        {
            var courseExists = await _db.Courses.AnyAsync(c => c.Id == dto.CourseId, ct);
            if (!courseExists)
                throw new KeyNotFoundException($"Course '{dto.CourseId}' was not found.");

            user.CourseId = dto.CourseId;
        }
        else
        {
            user.CourseId = null;
        }

        // Name update
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            var setNameResult = await _userManager.SetUserNameAsync(user, dto.Name);
            if (!setNameResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ", setNameResult.Errors.Select(e => e.Description)));
        }

        // Email update
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, dto.Email);
            if (!setEmailResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ", setEmailResult.Errors.Select(e => e.Description)));
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        // reload with course name
        await _db.Entry(user).Reference(u => u.Course).LoadAsync(ct);

        return await MapToUserDtoAsync(user);
    }

    public async Task<IEnumerable<UserDto>> GetTeachersByCourseAsync(int courseId, CancellationToken ct = default)
    {
        var course = await _db.Courses
            .Include(c => c.CourseTeachers)
                .ThenInclude(ct => ct.Teacher)
            .FirstOrDefaultAsync(c => c.Id == courseId, ct);

        if (course is null) return [];

        return course.CourseTeachers.Select(ct => new UserDto
        {
            Id = ct.TeacherId,
            Name = ct.Teacher?.UserName ?? string.Empty,
            Email = ct.Teacher?.Email ?? string.Empty,
            Role = "Teacher"
        });
    }
}
