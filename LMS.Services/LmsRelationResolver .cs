using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Services.Common;
using Service.Contracts;

namespace LMS.Services;

/// <summary>
/// Resolves course relationships for LMS entities.
/// </summary>
public class LmsRelationResolver : ILmsRelationResolver
{
    private readonly IUnitOfWork _unitOfWork;

    public LmsRelationResolver(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets a course by id.
    /// </summary>
    /// <param name="courseId">The course id.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The course.</returns>
    public async Task<Course> GetCourseByIdAsync(int courseId, CancellationToken ct = default)
    {
        return await _unitOfWork.Courses.GetCourseAsync(courseId)
            ?? throw new NotFoundException($"Course with id {courseId} was not found.");
    }

    /// <summary>
    /// Gets the course for a module.
    /// </summary>
    /// <param name="moduleId">The module id.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The related course.</returns>
    public async Task<Course> GetCourseFromModuleIdAsync(int moduleId, CancellationToken ct = default)
    {
        var module = await _unitOfWork.Modules.GetModuleAsync(moduleId)
            ?? throw new NotFoundException($"Module with id {moduleId} was not found.");

        return ResolveCourse(module);
    }

    /// <summary>
    /// Gets the course for an activity.
    /// </summary>
    /// <param name="activityId">The activity id.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The related course.</returns>
    public async Task<Course> GetCourseFromActivityIdAsync(int activityId, CancellationToken ct = default)
    {
        var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(activityId)
            ?? throw new NotFoundException($"Activity with id {activityId} was not found.");

        return ResolveCourse(activity);
    }

    /// <summary>
    /// Gets the course for a submission.
    /// </summary>
    /// <param name="submissionId">The submission id.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The related course.</returns>
    public async Task<Course> GetCourseFromSubmissionIdAsync(int submissionId, CancellationToken ct = default)
    {
        var submission = await _unitOfWork.Submissions.GetSubmissionWithRelationsAsync(submissionId)
            ?? throw new NotFoundException($"Submission with id {submissionId} was not found.");

        return ResolveCourse(submission);
    }

    /// <summary>
    /// Resolves the course for a module.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <returns>The related course.</returns>
    public Course ResolveCourse(Module module)
    {
        return NavProp.RequireLoadedFrom(module, (m => m.Course));
    }

    /// <summary>
    /// Resolves the course for an activity.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The related course.</returns>
    public Course ResolveCourse(Activity activity)
    {
        var module = NavProp.RequireLoadedFrom(activity, (a => a.Module));
        return ResolveCourse(module);
    }

    /// <summary>
    /// Resolves the course for a submission.
    /// </summary>
    /// <param name="submission">The submission.</param>
    /// <returns>The related course.</returns>
    public Course ResolveCourse(Submission submission)
    {
        var activity = NavProp.RequireLoadedFrom(submission, (s => s.Activity));
        return ResolveCourse(activity);
    }

    /// <summary>
    /// Resolves the course for a document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The related course.</returns>
    public Course ResolveCourse(Document document)
    {
        var course = NavProp.RequireLoadedIfForeignKeySet(
            entity: document,
            foreignKeySelector: (d => d.CourseId),
            navigationSelector: (d => d.Course));

        if (course != null)
        {
            return course;
        }

        var module = NavProp.RequireLoadedIfForeignKeySet(
            entity: document,
            foreignKeySelector: (d => d.ModuleId),
            navigationSelector: (d => d.Module));

        if (module != null)
        {
            return ResolveCourse(module);
        }

        var activity = NavProp.RequireLoadedIfForeignKeySet(
            entity: document,
            foreignKeySelector: (d => d.ActivityId),
            navigationSelector: (d => d.Activity));

        if (activity != null)
        {
            return ResolveCourse(activity);
        }

        var submission = NavProp.RequireLoadedIfForeignKeySet(
            entity: document,
            foreignKeySelector: (d => d.SubmissionId),
            navigationSelector: (d => d.Submission));

        if (submission != null)
        {
            return ResolveCourse(submission);
        }

        throw new InvalidOperationException(
            $"{nameof(Document)} with id {document.Id} is not linked to a {nameof(Course)}, {nameof(Module)}, {nameof(Activity)}, or {nameof(Submission)}.");
    }

    public async Task<Course> ResolveCourseForDocumentTargetAsync(
        int? courseId,
        int? moduleId,
        int? activityId,
        int? submissionId,
        CancellationToken ct = default)
    {
        if (courseId is not null)
        {
            return await _unitOfWork.Courses.GetCourseAsync(courseId.Value)
                ?? throw new NotFoundException($"Course with id {courseId.Value} was not found.");
        }

        if (moduleId is not null)
        {
            var module = await _unitOfWork.Modules.GetModuleAsync(moduleId.Value)
                ?? throw new NotFoundException($"Module with id {moduleId.Value} was not found.");

            return ResolveCourse(module);
        }

        if (activityId is not null)
        {
            var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(activityId.Value)
                ?? throw new NotFoundException($"Activity with id {activityId.Value} was not found.");

            return ResolveCourse(activity);
        }

        if (submissionId is not null)
        {
            var submission = await _unitOfWork.Submissions.GetSubmissionWithRelationsAsync(submissionId.Value)
                ?? throw new NotFoundException($"Submission with id {submissionId.Value} was not found.");

            return ResolveCourse(submission);
        }

        throw new BadRequestException(
            "Document must belong to a course, module, activity, or submission.");
    }

}