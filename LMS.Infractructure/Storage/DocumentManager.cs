using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LMS.Infrastructure.Storage;

public class DocumentManager(
    IFileStorage fileStorage,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : IDocumentManager
{
    public async Task<Document> CreateEntityAsync(
        string userId,
        int? courseId = null,
        int? moduleId = null,
        int? activityId = null,
        int? submissionId = null,
        string? description = null)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException($"User by id {userId} does not exist");

        return new Document
        {
            UploadedByUserId = userId,
            CourseId = courseId,
            ModuleId = moduleId,
            ActivityId = activityId,
            SubmissionId = submissionId,
            Description = description ?? string.Empty
        };
    }

    public async Task AttachFileAsync(Document document, IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (file == null)
        {
            throw new BadRequestException("File is missing.");
        }

        var savedFileResult = await fileStorage.SaveAsync(file);

        document.StoredFileName = savedFileResult.FileName;
        document.FileSize = savedFileResult.FileSize;
        document.ContentType = file.ContentType;
        document.DisplayName = file.FileName;
        document.UploadedAt = DateTime.Now;
    }

    public async Task SaveDocumentAsync(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        unitOfWork.Documents.Create(document);
        await unitOfWork.CompleteAsync();
    }

    public async Task ReplaceForSubmissionAsync(Submission submission, IFormFile file, string? fileDescription)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (file == null)
        {
            throw new BadRequestException("File is missing.");
        }

        await RemoveFromSubmissionAsync(submission);

        var document = await CreateEntityAsync(
            submission.StudentId,
            submissionId: submission.Id,
            description: fileDescription);

        await AttachFileAsync(document, file);

        submission.Document = document;

        await unitOfWork.CompleteAsync();
    }

    public async Task RemoveFromSubmissionAsync(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (submission.Document == null)
        {
            return;
        }

        var document = submission.Document;

        unitOfWork.Documents.Delete(document);
        submission.Document = null;

        await DeleteFileAsync(document);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteFileAsync(Document document)
    {
        await fileStorage.DeleteAsync(document.StoredFileName);
    }

    public async Task DeleteManyAsync(IEnumerable<Document> documents)
    {
        ArgumentNullException.ThrowIfNull(documents);

        foreach (var document in documents)
        {
            await DeleteFileAsync(document);
            unitOfWork.Documents.Delete(document);
        }

        await unitOfWork.CompleteAsync();
    }

    public async Task<Stream> OpenReadAsync(Document document)
    {
        return await fileStorage.OpenReadAsync(document.StoredFileName);
    }
}