using Domain.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts.Storage;

/// <summary>
/// Provides helper methods for creating, attaching files to, persisting,
/// replacing, and removing <see cref="Document"/> entities.
/// </summary>
public interface IDocumentManager
{
    /// <summary>
    /// Creates a new <see cref="Document"/> entity with the provided ownership
    /// relation and optional description, but does not attach any file or save
    /// the entity to the database.
    /// </summary>
    /// <param name="userId">
    /// The id of the user who uploads and owns the document metadata as uploader.
    /// </param>
    /// <param name="courseId">
    /// The id of the related course, if the document belongs to a course.
    /// </param>
    /// <param name="moduleId">
    /// The id of the related module, if the document belongs to a module.
    /// </param>
    /// <param name="activityId">
    /// The id of the related activity, if the document belongs to an activity.
    /// </param>
    /// <param name="submissionId">
    /// The id of the related submission, if the document belongs to a submission.
    /// </param>
    /// <param name="description">
    /// An optional description to store on the document.
    /// </param>
    /// <returns>
    /// A new <see cref="Document"/> entity with uploader and ownership fields populated.
    /// </returns>
    Task<Document> CreateEntityAsync(
        string userId,
        int? courseId = null,
        int? moduleId = null,
        int? activityId = null,
        int? submissionId = null,
        string? description = null);

    /// <summary>
    /// Saves the provided file to storage and copies its file metadata to the
    /// given <see cref="Document"/> entity, but does not save the document to the database.
    /// </summary>
    /// <param name="document">
    /// The document entity to update with file metadata.
    /// </param>
    /// <param name="file">
    /// The uploaded file to attach to the document.
    /// </param>
    Task AttachFileAsync(Document document, IFormFile file);

    /// <summary>
    /// Persists the provided <see cref="Document"/> entity to the database.
    /// </summary>
    /// <param name="document">
    /// The document entity to save.
    /// </param>
    Task SaveDocumentAsync(Document document);

    /// <summary>
    /// Replaces the current document attached to the given submission with a new one
    /// created from the provided file.
    /// </summary>
    /// <param name="submission">
    /// The submission whose document should be replaced.
    /// </param>
    /// <param name="file">
    /// The new file to attach to the submission.
    /// </param>
    Task ReplaceForSubmissionAsync(Submission submission, IFormFile file, string? fileDescription);

    /// <summary>
    /// Removes the document currently attached to the given submission.
    /// </summary>
    /// <param name="submission">
    /// The submission whose attached document should be removed.
    /// </param>
    Task RemoveFromSubmissionAsync(Submission submission);

    Task DeleteFileAsync(Document document);
    Task DeleteManyAsync(IEnumerable<Document> documents);
    Task<Stream> OpenReadAsync(Document document);
}