using Domain.Models.Entities;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;

public class StudentCourseService : IStudentCourseService
{
    private const string StudentRole = "Student";

    private readonly UserManager<ApplicationUser> userManager;
    private readonly ICourseService courseService;

    public StudentCourseService(UserManager<ApplicationUser> userManager, ICourseService courseService)
    {
        this.userManager = userManager;
        this.courseService = courseService;
    }

    public async Task<CourseDto?> GetStudentCourseAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        if (!await userManager.IsInRoleAsync(user, StudentRole))
            return null;

        if (user.CourseId is not int courseId)
            return null;

        return await courseService.GetCourseByIdAsync(courseId);
    }

    public async Task<IReadOnlyList<StudentDto>> GetCourseClassmatesAsync(int courseId, string excludeUserId)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .Where(u => u.CourseId == courseId && u.Id != excludeUserId)
            .ToListAsync();

        return users
            .Select(u => new StudentDto
            {
                Id = u.Id,
                Name = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty
            })
            .ToList();
    }
}
