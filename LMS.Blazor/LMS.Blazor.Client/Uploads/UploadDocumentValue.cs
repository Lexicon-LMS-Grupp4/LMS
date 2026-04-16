using BlazorBlueprint.Components;

namespace LMS.Blazor.Client.Uploads;

public class UploadDocumentValue
{
    public FileUploadItem? File { get; set; }
    public string Description { get; set; } = string.Empty;

    public int? ExistingDocumentId { get; set; }
    public string? ExistingFileName { get; set; }
    public string? ExistingContentType { get; set; }

    // flag to use when to delete document, since we don't have proper patch endpoint! (File = null results in no change, not file deletion)
    // TODO: implement (add RemoveFile flag in UpdateSubmissionDto, so we can call DeleteDocument)
    public bool RemoveExistingDocument { get; set; }

    public bool HasFile => File is not null;
    public bool HasExistingDocument => ExistingDocumentId is not null;

}