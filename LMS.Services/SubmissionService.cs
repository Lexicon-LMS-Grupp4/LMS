using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Services.Access;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;
using LMS.Shared.Request;
using Microsoft.AspNetCore.Http;
using Service.Contracts;

namespace LMS.Services;

/// <summary>
/// Submission service implementation
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDocumentManager documentManager;
    private readonly ILmsAccessService lmsAccessService;
    private readonly IUserAccessContextFactory userAccessContextFactory;
    public SubmissionService(IUnitOfWork unitOfWork,
                             IMapper mapper,
                             ILmsAccessService lmsAccessService,
                             IUserAccessContextFactory userAccessContextFactor,
                             IDocumentManager documentManager)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.lmsAccessService = lmsAccessService;
        this.userAccessContextFactory = userAccessContextFactor;
        this.documentManager = documentManager;
    }
    public async Task<PagedResultDto<SubmissionDto>> GetAllSubmissionsAsync(string userId, SubmissionsRequestParams queryDto)
    {
        if (queryDto.ActivityId != null)
        {
            var activity = await unitOfWork.Activities.GetActivityWithRelationsAsync(queryDto.ActivityId.Value)
                ?? throw new NotFoundException($"Activity with id {queryDto.ActivityId.Value} was not found.");
            await lmsAccessService.EnsureCanAccessActivityAsync(userId, activity);
        }

        var access = await userAccessContextFactory.CreateAsync(userId);

        var query = lmsAccessService.ApplySubmissionAccessFilter(
            unitOfWork.Submissions.BuildQuery(queryDto.ActivityId, queryDto.StudentId),
            access);

        var (submissions, totalCount) = await query
            .OrderBy(s => s.SubmittedAt)
            .PagedResult(queryDto.Page, queryDto.PageSize);

        return new PagedResultDto<SubmissionDto>
        {
            Items = mapper.Map<List<SubmissionDto>>(submissions),
            TotalCount = totalCount,
            PageNumber = queryDto.Page,
            PageSize = queryDto.PageSize
        };

    }

    public async Task<SubmissionDto> GetSubmissionByIdAsync(int id, string userId)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionWithRelationsAsync(id)
            ?? throw new NotFoundException($"Submission with id {id} was not found.");

        await lmsAccessService.EnsureCanAccessSubmissionAsync(userId, submission);

        return mapper.Map<SubmissionDto>(submission);
    }
    public async Task<SubmissionDetailDto> GetSubmissionDetailByIdAsync(int id, string userId)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionWithRelationsAsync(id)
            ?? throw new NotFoundException($"Submission with id {id} was not found.");

        await lmsAccessService.EnsureCanAccessSubmissionAsync(userId, submission);

        return mapper.Map<SubmissionDetailDto>(submission);
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(string userId, CreateSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var activity = await unitOfWork.Activities.GetActivityWithRelationsAsync(dto.ActivityId)
            ?? throw new NotFoundException($"Activity with id {dto.ActivityId} was not found.");

        await lmsAccessService.EnsureCanAccessActivityAsync(userId, activity);

        var existingSubmission = await unitOfWork.Submissions.GetByActivityAndStudentAsync(dto.ActivityId, userId, trackChanges: true);
        if (existingSubmission is not null)
        {
            var timelineComment = BuildResubmissionTimelineComment(existingSubmission, dto.Body, dto.File, dto.RemoveFile);

            existingSubmission.Body = dto.Body;
            existingSubmission.SubmittedAt = DateTime.Now;
            existingSubmission.IsLate = activity.EndTime < DateTime.Now;

            if (dto.File != null)
            {
                await documentManager.ReplaceForSubmissionAsync(existingSubmission, dto.File, dto.FileDescription);
            }
            else if (dto.RemoveFile)
            {
                await documentManager.RemoveFromSubmissionAsync(existingSubmission);
                await unitOfWork.CompleteAsync();
            }
            else
            {
                await unitOfWork.CompleteAsync();
            }

            if (!string.IsNullOrWhiteSpace(timelineComment))
            {
                existingSubmission.Comments.Add(SubmissionComment.CreateNew(existingSubmission.Id, userId, timelineComment));
                await unitOfWork.CompleteAsync();
            }

            return mapper.Map<SubmissionDto>(existingSubmission);
        }

        var submission = mapper.Map<Submission>(dto);
        submission.StudentId = userId;
        submission.SubmittedAt = DateTime.Now;
        submission.IsLate = activity.EndTime < DateTime.Now;

        unitOfWork.Submissions.Create(submission);
        await unitOfWork.CompleteAsync();

        // refetch new submission with navprops to verify it exists
        var createdSubmission = await unitOfWork.Submissions.GetSubmissionAsync(submission.Id, trackChanges: true)
            ?? throw new InvalidOperationException($"Submission with id {submission.Id} could not be loaded after creation.");

        if (dto.File != null)
        {
            var document = await documentManager.CreateEntityAsync(
                userId,
                submissionId: createdSubmission.Id,
                description: dto.FileDescription);

            await documentManager.AttachFileAsync(document, dto.File);
            await documentManager.SaveDocumentAsync(document);

            createdSubmission.Document = document;
            await unitOfWork.CompleteAsync();
        }

        return mapper.Map<SubmissionDto>(createdSubmission);
    }
    public async Task UpdateSubmissionAsync(int id, string userId, UpdateSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var submission = await GetStudentAccessedSubmission(id, userId);

        await InternallyUpdateSubmissionAsync(submission, dto.Body, dto.ActivityId, dto.File, dto.FileDescription, dto.RemoveFile);
    }

    public async Task UpdateSubmissionPartiallyAsync(int id, string userId, PatchSubmissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var submission = await GetStudentAccessedSubmission(id, userId);

        await InternallyUpdateSubmissionAsync(submission,
                                              dto.Body ?? submission.Body ?? string.Empty,
                                              dto.ActivityId ?? submission.Activity.Id,
                                              dto.File ?? null,
                                              dto.FileDescription ?? submission.Document?.Description,
                                              false);
    }
    private async Task InternallyUpdateSubmissionAsync(Submission submission, string? body, int? activityId, IFormFile? file, string? fileDescription, bool removeFile)
    {
        var timelineComment = BuildResubmissionTimelineComment(submission, body, file, removeFile);

        submission.Body = body ?? submission.Body;
        submission.ActivityId = activityId ?? submission.ActivityId;
        if (file != null)
        {
            await documentManager.ReplaceForSubmissionAsync(submission, file, fileDescription);
        }
        else if (removeFile && submission.Document is not null)
        {
            await documentManager.RemoveFromSubmissionAsync(submission);
            await unitOfWork.CompleteAsync();
        }
        else
        {
            await unitOfWork.CompleteAsync();
        }

        if (!string.IsNullOrWhiteSpace(timelineComment))
        {
            submission.Comments.Add(SubmissionComment.CreateNew(submission.Id, submission.StudentId, timelineComment));
            await unitOfWork.CompleteAsync();
        }
    }
    public async Task DeleteSubmissionAsync(int id, string userId)
    {
        var submission = await GetStudentAccessedSubmission(id, userId);

        await documentManager.RemoveFromSubmissionAsync(submission);

        unitOfWork.Submissions.Delete(submission);
        await unitOfWork.CompleteAsync();
    }

    public async Task SubmitCommentAsync(int submissionId, string userId, SubmitCommentDto dto)
    {
        var submission = await GetStudentAccessedSubmission(submissionId, userId);

        submission.Comments.Add(SubmissionComment.CreateNew(submission.Id, userId, dto.CommentText));

        await unitOfWork.CompleteAsync();
    }

    private async Task<Submission> GetStudentAccessedSubmission(int id, string userId)
    {
        var submission = await unitOfWork.Submissions.GetSubmissionWithRelationsAsync(id, trackChanges: true)
            ?? throw new NotFoundException($"Submission with id {id} does not exist.");
        await lmsAccessService.EnsureCanAccessSubmissionAsync(userId, submission);
        return submission;
    }

    public async Task DeleteCommentAsync(int commentId, string userId)
    {
        // TODO: implement if needed
        //var submission = await unitOfWork.Submissions.GetSubmissionAsync(submissionId, trackChanges: true) ??
        //    throw new KeyNotFoundException($"Submission with id {submissionId} does not exist.");

        //await ThrowIfNotAuthorizedToComment(submission, userId);
    }

    private static string? BuildResubmissionTimelineComment(Submission submission, string? newBody, IFormFile? newFile, bool removeFile)
    {
        var previousBody = submission.Body?.Trim();
        var nextBody = newBody?.Trim();

        var previousFileName = submission.Document?.DisplayName;
        var nextFileName = removeFile
            ? null
            : newFile?.FileName ?? previousFileName;

        var bodyChanged = !string.Equals(previousBody, nextBody, StringComparison.Ordinal);
        var fileChanged = !string.Equals(previousFileName, nextFileName, StringComparison.Ordinal);

        if (!bodyChanged && !fileChanged)
        {
            return null;
        }

        return string.Join(Environment.NewLine, [
            "Student resubmitted",
            string.Empty,
            "Previous submission",
            $"Text: {FormatTimelineValue(previousBody)}",
            $"File: {FormatTimelineValue(previousFileName)}",
            string.Empty,
            "New submission",
            $"Text: {FormatTimelineValue(nextBody)}",
            $"File: {FormatTimelineValue(nextFileName)}"
        ]);
    }

    private static string FormatTimelineValue(string? value)
        => string.IsNullOrWhiteSpace(value) ? "(none)" : value.Trim();
}
