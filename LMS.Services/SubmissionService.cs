using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;
using Service.Contracts;

namespace LMS.Services;

/// <summary>
/// Submission service implementation
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }
    public async Task<PagedResultDto<SubmissionDto>> GetAllSubmissionsAsync(int page,
                                                                            int pageSize,
                                                                            int? activityId = null,
                                                                            string? studentId = null)
    {
        var (submissions, totalCount) = await unitOfWork.Submissions.GetAllSubmissionsAsync(page, pageSize, activityId, studentId);

        return new PagedResultDto<SubmissionDto>
        {
            Items = mapper.Map<List<SubmissionDto>>(submissions),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<SubmissionDto> GetSubmissionByIdAsync(int id)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionAsync(id)
            ?? throw new NotFoundException($"Submission with id {id} was not found.");

        return mapper.Map<SubmissionDto>(submission);
    }
    public async Task<SubmissionDetailDto> GetSubmissionDetailByIdAsync(int id)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionAsync(id)
            ?? throw new NotFoundException($"Submission with id {id} was not found.");
        return mapper.Map<SubmissionDetailDto>(submission);
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(string userId, CreateSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var activity = await unitOfWork.Activities.FindByIdOrThrowAsync(dto.ActivityId);

        var submission = mapper.Map<Submission>(dto);
        submission.StudentId = userId;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.IsLate = activity.EndTime < DateTime.UtcNow;

        await ThrowIfNotValidSubmission(submission);

        unitOfWork.Submissions.Create(submission);
        await unitOfWork.CompleteAsync();

        // refetch new submission with navprops to verify it exists
        var createdSubmission = await unitOfWork.Submissions.GetSubmissionAsync(submission.Id)
            ?? throw new InvalidOperationException($"Submission with id {submission.Id} could not be loaded after creation.");

        return mapper.Map<SubmissionDto>(createdSubmission);
    }

    private async Task ThrowIfNotValidSubmission(Submission submission)
    {
        await ThrowIfNotValidSubmission(submission.ActivityId, submission.StudentId);
    }

    private async Task ThrowIfNotValidSubmission(int activityId, string submissionUserId)
    {
        // TODO: uncomment and implement properly when we have users and activities working with modules and courses
        //var activity = await unitOfWork.Activities.GetActivityAsync(activityId) ??
        //    throw new KeyNotFoundException($"Activity with id {activityId} was not found.");

        //ApplicationUser user = await unitOfWork.Users.GetUserAsync(submissionUserId);
        //if (user == null)
        //{
        //    throw new KeyNotFoundException($"User with id {submissionUserId} does not exist.");
        //}
        //if (user.CourseId != activity.Module.CourseId) // TODO: only works if activity includes module and course
        //{
        //    throw new ForbiddenException($"User with id {submissionUserId} is not enrolled in the course for this activity.");
        //}
    }



    public async Task UpdateSubmissionAsync(int id, UpdateSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var submission = await unitOfWork.Submissions.FindByIdOrThrowAsync(id, trackChanges: true);

        await InternallyUpdateSubmissionAsync(submission, dto.Body, dto.ActivityId, dto.DocumentId);
    }

    public async Task UpdateSubmissionPartiallyAsync(int id, PatchSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var submission = await unitOfWork.Submissions.GetSubmissionAsync(id, trackChanges: true) ??
            throw new KeyNotFoundException($"Submission with id {id} does not exist.");

        await InternallyUpdateSubmissionAsync(submission,
                                              dto.Body ?? submission.Body ?? string.Empty,
                                              dto.ActivityId ?? submission.Activity.Id,
                                              dto.DocumentId);
    }
    private async Task InternallyUpdateSubmissionAsync(Submission submission, string body, int activityId, int? documentId)
    {
        submission.Body = body;
        if (documentId != null)
        {
            submission.Document = await unitOfWork.Documents.FindByIdOrThrowAsync(documentId.Value, trackChanges: true);
        }
        submission.ActivityId = activityId;

        await ThrowIfNotValidSubmission(submission);

        await unitOfWork.CompleteAsync();
    }
    public async Task DeleteSubmissionAsync(int id)
    {
        var submission = await unitOfWork.Submissions.FindByIdOrThrowAsync(id, trackChanges: true);

        unitOfWork.Submissions.Delete(submission);
        await unitOfWork.CompleteAsync();
    }

    public async Task SubmitCommentAsync(int submissionId, string commenterId, SubmitCommentDto dto)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionAsync(submissionId, trackChanges: true) ??
            throw new KeyNotFoundException($"Submission with id {submissionId} does not exist.");

        await ThrowIfNotAuthorizedToComment(submission, commenterId);

        submission.Comments.Add(SubmissionComment.CreateNew(submission.Id, commenterId, dto.CommentText));

        await unitOfWork.CompleteAsync();
    }

    private async Task ThrowIfNotAuthorizedToComment(Submission submission, string commenterId)
    {
        // TODO: uncomment and implement properly when we have users and activities working with modules and courses
        //ApplicationUser user = await unitOfWork.Users.GetUserAsync(commenterId) ??
        //    throw new KeyNotFoundException($"User with id {commenterId} does not exist.");

        //Activity activity = await unitOfWork.Activities.GetActivityAsync(submission.ActivityId) ??
        //    throw new KeyNotFoundException($"Activity with id {submission.ActivityId} was not found.");
        //bool isOwner = submission.StudentId == commenterId;
        //bool isTeacherOnCourse = user.TeachingCourses.Any(tc => tc.CourseId == activity.Module.CourseId);

        //if (!isOwner && !isTeacherOnCourse)
        //{
        //    throw new ForbiddenException($"User not allowed to comment this submission");
        //}
    }

    public async Task DeleteCommentAsync(int commentId, string commenterId)
    {
        // TODO: implement if needed
        //var submission = await unitOfWork.Submissions.GetSubmissionAsync(submissionId, trackChanges: true) ??
        //    throw new KeyNotFoundException($"Submission with id {submissionId} does not exist.");

        //await ThrowIfNotAuthorizedToComment(submission, commenterId);
    }
}