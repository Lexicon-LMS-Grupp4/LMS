using Domain.Models.Entities;

namespace Service.Contracts;

public interface ILmsRelationResolver
{
    Task<Course> GetCourseByIdAsync(int courseId, CancellationToken ct = default);
    Task<Course> GetCourseFromModuleIdAsync(int moduleId, CancellationToken ct = default);
    Task<Course> GetCourseFromActivityIdAsync(int activityId, CancellationToken ct = default);
    Task<Course> GetCourseFromSubmissionIdAsync(int submissionId, CancellationToken ct = default);

    Course ResolveCourse(Module module);
    Course ResolveCourse(Activity activity);
    Course ResolveCourse(Submission submission);
    Course ResolveCourse(Document document);
    Task<Course> ResolveCourseForDocumentTargetAsync(
        int? courseId,
        int? moduleId,
        int? activityId,
        int? submissionId,
        CancellationToken ct = default);
}