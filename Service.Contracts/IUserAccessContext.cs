namespace Service.Contracts;

public interface IUserAccessContext
{
    string UserId { get; }
    bool IsAdmin { get; }
    bool IsTeacher { get; }
    bool IsStudent { get; }

    int? StudentCourseId { get; }
    IReadOnlySet<int> TeachingCourseIds { get; }
    bool HasCourseAccess(int courseId);
}