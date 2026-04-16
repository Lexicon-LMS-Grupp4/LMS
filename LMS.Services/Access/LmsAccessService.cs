using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Services.Common;
using Service.Contracts;

namespace LMS.Services.Access;

/// <summary>
/// Central service for LMS access control.
/// </summary>
public class LmsAccessService : ILmsAccessService
{
    private readonly IUserAccessContextFactory userAccessContextFactory;
    private readonly ILmsRelationResolver lmsRelationResolver;

    public LmsAccessService(
        IUserAccessContextFactory userAccessContextFactory,
        ILmsRelationResolver lmsRelationResolver)
    {
        this.userAccessContextFactory = userAccessContextFactory;
        this.lmsRelationResolver = lmsRelationResolver;
    }

    /// <summary>
    /// Ensures teacher-level access to a course.
    /// </summary>
    public async Task EnsureTeacherForCourseAsync(string userId, Course course, CancellationToken ct = default)
    {
        await EnsureTeacherForCourseAsync(userId, course.Id, ct);
    }

    public async Task EnsureTeacherForCourseAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);

        if (access.IsAdmin || access.IsTeacher)
            return;

        throw new ForbiddenException("You do not have teacher access to this course.");
    }

    /// <summary>
    /// Ensures access to a course.
    /// </summary>
    public async Task EnsureCanAccessCourseAsync(string userId, Course course, CancellationToken ct = default)
    {
        await EnsureCanAccessCourseAsync(userId, course.Id, ct);
    }
    public async Task EnsureCanAccessCourseAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanAccessCourse(access, courseId);
    }

    /// <summary>
    /// Ensures access to a module.
    /// </summary>
    public async Task EnsureCanAccessModuleAsync(string userId, Module module, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        //EnsureCanAccessModule(access, module);
    }

    /// <summary>
    /// Ensures access to an activity.
    /// </summary>
    public async Task EnsureCanAccessActivityAsync(string userId, Activity activity, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanAccessActivity(access, activity);
    }

    /// <summary>
    /// Ensures access to a submission, i.e the author student and teachers for the course the submission belongs to.
    /// </summary>
    public async Task EnsureCanAccessSubmissionAsync(string userId, Submission submission, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanAccessSubmission(access, submission);
    }

    /// <summary>
    /// Ensures access to a document, i.e the student uploading it and teachers for the course the document belongs to.
    /// </summary>
    public async Task EnsureCanAccessDocumentAsync(string userId, Document document, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanAccessDocument(access, document);
    }

    /// <summary>
    /// Ensures that the user may modify the specified document.
    /// </summary>
    public async Task EnsureCanModifyDocumentAsync(string userId, Document document, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanModifyDocument(access, document);
    }

    /// <summary>
    /// Ensures that the user may delete the specified document.
    /// </summary>
    public async Task EnsureCanDeleteDocumentAsync(string userId, Document document, CancellationToken ct = default)
    {
        var access = await userAccessContextFactory.CreateAsync(userId, ct);
        EnsureCanDeleteDocument(access, document);
    }

    /// <summary>
    /// Applies course access filtering to a query.
    /// </summary>

    public IQueryable<Course> ApplyCourseAccessFilter(IQueryable<Course> query, IUserAccessContext access)
    {
        if (access.IsAdmin || access.IsTeacher)
            return query;

        return query
            .WhereIf(access.IsStudent && access.StudentCourseId is not null,
                c => c.Id == access.StudentCourseId!.Value);
    }

    /// <summary>
    /// Applies module access filtering to a query.
    /// </summary>
    public IQueryable<Module> ApplyModuleAccessFilter(IQueryable<Module> query, IUserAccessContext access)
    {
        if (access.IsAdmin)
            return query;

        return query
            .WhereIf(access.IsTeacher,
                m => true) // Teachers can see all modules
            .WhereIf(access.IsStudent && access.StudentCourseId is not null,
                m => m.CourseId == access.StudentCourseId!.Value);
    }

    /// <summary>
    /// Applies activity access filtering to a query.
    /// </summary>
    public IQueryable<Activity> ApplyActivityAccessFilter(IQueryable<Activity> query, IUserAccessContext access)
    {
        if (access.IsAdmin)
            return query;

        return query
            .WhereIf(access.IsTeacher,
                a => true) // Teachers can see all activities
            .WhereIf(access.IsStudent && access.StudentCourseId is not null,
                a => a.Module != null && a.Module.CourseId == access.StudentCourseId!.Value);
    }

    /// <summary>
    /// Applies submission access filtering to a query.
    /// </summary>
    public IQueryable<Submission> ApplySubmissionAccessFilter(IQueryable<Submission> query, IUserAccessContext access)
    {
        if (access.IsAdmin)
            return query;

        if (access.IsTeacher)
        {
            return query.Where(s =>
                s.Activity != null &&
                s.Activity.Module != null &&
                access.TeachingCourseIds.Contains(s.Activity.Module.CourseId));
        }

        if (access.IsStudent && access.StudentCourseId is not null)
        {
            var studentCourseId = access.StudentCourseId.Value;

            return query.Where(s =>
                s.StudentId == access.UserId &&
                s.Activity != null &&
                s.Activity.Module != null &&
                s.Activity.Module.CourseId == studentCourseId);
        }

        return query.Where(_ => false);
    }

    /// <summary>
    /// Applies document access filtering to a query.
    /// </summary>
    public IQueryable<Document> ApplyDocumentAccessFilter(IQueryable<Document> query, IUserAccessContext access)
    {
        if (access.IsAdmin)
            return query;

        if (access.IsTeacher)
        {
            return query; // Teachers can access all documents
        }

        if (access.IsStudent && access.StudentCourseId is not null)
        {
            var studentCourseId = access.StudentCourseId.Value;

            return query.Where(d =>
                d.UploadedByUserId == access.UserId ||

                (d.Submission != null &&
                 d.Submission.StudentId == access.UserId &&
                 d.Submission.Activity != null &&
                 d.Submission.Activity.Module != null &&
                 d.Submission.Activity.Module.CourseId == studentCourseId) ||

                (d.CourseId.HasValue &&
                 d.CourseId.Value == studentCourseId) ||

                (d.Module != null &&
                 d.Module.CourseId == studentCourseId) ||

                (d.Activity != null &&
                 d.Activity.Module != null &&
                 d.Activity.Module.CourseId == studentCourseId));
        }

        return query.Where(_ => false);
    }

    private void EnsureCanAccessCourse(IUserAccessContext access, Course course)
    {
        EnsureCanAccessCourse(access, course.Id);
    }
    private void EnsureCanAccessCourse(IUserAccessContext access, int courseId)
    {
        if (access.IsAdmin || access.IsTeacher)
            return;

        if (access.HasCourseAccess(courseId))
            return;

        throw new ForbiddenException("You do not have access to this course.");
    }

    private void EnsureCanAccessModule(IUserAccessContext access, Module module)
    {
        var course = lmsRelationResolver.ResolveCourse(module);

        if (access.HasCourseAccess(course.Id))
            return;

        throw new ForbiddenException("You do not have access to this module.");
    }

    private void EnsureCanAccessActivity(IUserAccessContext access, Activity activity)
    {
        var course = lmsRelationResolver.ResolveCourse(activity);

        if (access.HasCourseAccess(course.Id))
            return;

        throw new ForbiddenException("You do not have access to this activity.");
    }

    private void EnsureCanAccessSubmission(IUserAccessContext access, Submission submission)
    {
        if (access.IsAdmin)
            return;

        if (submission.StudentId == access.UserId)
            return;

        var course = lmsRelationResolver.ResolveCourse(submission);

        if (access.IsTeacher) // Teacher can access any submission (or change this if required, but user requested 'teacher should have access to all submissions')
            return;

        throw new ForbiddenException("You do not have access to this submission.");
    }

    private void EnsureCanAccessDocument(IUserAccessContext access, Document document)
    {
        if (access.IsAdmin)
            return;

        if (document.UploadedByUserId == access.UserId)
            return;

        if (document.SubmissionId is not null)
        {
            var submission = NavProp.RequireLoadedFrom(document, x => x.Submission);

            if (submission.StudentId == access.UserId)
                return;

            var submissionCourse = lmsRelationResolver.ResolveCourse(submission);

            if (access.IsTeacher)
                return;

            throw new ForbiddenException("You do not have access to this document.");
        }

        var course = lmsRelationResolver.ResolveCourse(document);

        if (access.HasCourseAccess(course.Id))
            return;

        throw new ForbiddenException("You do not have access to this document.");
    }

    private void EnsureCanModifyDocument(IUserAccessContext access, Document document)
    {
        if (access.IsAdmin)
            return;

        if (document.UploadedByUserId == access.UserId)
            return;

        if (document.SubmissionId is not null)
        {
            var submission = NavProp.RequireLoadedFrom(document, x => x.Submission);

            if (submission.StudentId == access.UserId)
                return;

            var submissionCourse = lmsRelationResolver.ResolveCourse(submission);

            if (access.IsTeacher && access.TeachingCourseIds.Contains(submissionCourse.Id))
                return;

            throw new ForbiddenException("You do not have permission to modify this document.");
        }

        var course = lmsRelationResolver.ResolveCourse(document);

        if (access.IsTeacher && access.TeachingCourseIds.Contains(course.Id))
            return;

        throw new ForbiddenException("You do not have permission to modify this document.");
    }

    private void EnsureCanDeleteDocument(IUserAccessContext access, Document document)
    {
        EnsureCanModifyDocument(access, document);
    }
}