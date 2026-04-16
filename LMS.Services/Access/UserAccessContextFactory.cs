using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services.Access;

public class UserAccessContextFactory : IUserAccessContextFactory
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public UserAccessContextFactory(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IUserAccessContext> CreateAsync(
        string userId,
        CancellationToken ct = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(x => x.TeachingCourses)
            .FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new UnauthorizedAccessException($"User '{userId}' not found.");

        return await CreateAsync(user, ct);
    }

    public async Task<IUserAccessContext> CreateAsync(
        ApplicationUser user,
        CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var isAdmin = roles.Contains("Admin");
        var isTeacher = roles.Contains("Teacher");
        var isStudent = roles.Contains("Student");

        HashSet<int> teachingCourseIds = new();

        if (isTeacher)
        {
            teachingCourseIds = await _db.Courses
                .AsNoTracking()
                .Where(c => c.CourseTeachers != null && c.CourseTeachers.Any(ct => ct.TeacherId == user.Id))
                .Select(c => c.Id)
                .ToHashSetAsync(ct);

        }

        return new UserAccessContext
        {
            UserId = user.Id,
            IsAdmin = isAdmin,
            IsTeacher = isTeacher,
            IsStudent = isStudent,
            StudentCourseId = isStudent ? user.CourseId : null,
            TeachingCourseIds = teachingCourseIds
        };
    }
}