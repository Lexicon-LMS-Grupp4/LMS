using Service.Contracts;

namespace LMS.Services.Access;

public record UserAccessContext : IUserAccessContext
{
    public required string UserId { get; init; }

    public bool IsAdmin { get; init; }
    public bool IsTeacher { get; init; }
    public bool IsStudent { get; init; }

    public int? StudentCourseId { get; init; }
    public IReadOnlySet<int> TeachingCourseIds { get; init; } = new HashSet<int>();

    public bool HasCourseAccess(int courseId)
    {
        if (IsAdmin) return true;
        if (IsTeacher) return true;  // Teachers have access to all courses
        if (IsStudent && StudentCourseId == courseId) return true;

        return false;
    }
}