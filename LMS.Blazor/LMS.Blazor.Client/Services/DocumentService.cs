using LMS.Blazor.Client.Uploads;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Document;

namespace LMS.Blazor.Client.Services;

public class DocumentService : IDocumentService
{
    private readonly IApiService _apiService;
    private readonly ILogger<DocumentService> _logger;

    private const string Base = "api/documents";

    public DocumentService(IApiService apiService, ILogger<DocumentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResultDto<DocumentDto>?> GetAllDocumentsAsync(
        int page = 1,
        int pageSize = 10,
        DocumentQueryDto? query = null,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"{Base}?page={page}&pageSize={pageSize}";

            if (query?.CourseId.HasValue == true)
                url += $"&courseId={query.CourseId.Value}";

            if (query?.ModuleId.HasValue == true)
                url += $"&moduleId={query.ModuleId.Value}";

            if (query?.ActivityId.HasValue == true)
                url += $"&activityId={query.ActivityId.Value}";

            if (query?.SubmissionId.HasValue == true)
                url += $"&submissionId={query.SubmissionId.Value}";

            if (!string.IsNullOrEmpty(query?.ScopeTarget))
                url += $"&scopeTarget={query.ScopeTarget}";

            return await _apiService.GetAsync<PagedResultDto<DocumentDto>>(url, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching documents.");
            return null;
        }
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<DocumentDto>($"{Base}/{id}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching document {DocumentId}.", id);
            return null;
        }
    }

    public async Task<DocumentDto?> CreateDocumentAsync(
        CreateDocumentDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default)
    {
        try
        {
            using var content = BuildCreateDocumentContent(dto, document);
            return await _apiService.PostMultipartAsync<DocumentDto>(Base, content, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document.");
            return null;
        }
    }

    public async Task<DocumentDto?> UpdateDocumentAsync(
        int id,
        UpdateDocumentDto dto,
        CancellationToken ct = default)
    {
        try
        {
            await _apiService.PutAsync<DocumentDto>($"{Base}/{id}", dto, ct);
            return await GetDocumentByIdAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document {DocumentId}.", id);
            return null;
        }
    }

    public async Task DeleteDocumentAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _apiService.DeleteAsync($"{Base}/{id}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}.", id);
        }
    }

    public string GetDocumentFileUrl(int id, bool download = false)
    {
        return $"/api/proxy/{Base}/{id}/file?download={download.ToString().ToLowerInvariant()}";
    }

    private static MultipartFormDataContent BuildCreateDocumentContent(
        CreateDocumentDto dto,
        UploadDocumentValue? document)
    {
        var content = new MultipartFormDataContent();

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            content.Add(new StringContent(dto.Description), "Description");
        }

        if (dto.CourseId.HasValue)
        {
            content.Add(new StringContent(dto.CourseId.Value.ToString()), "CourseId");
        }

        if (dto.ModuleId.HasValue)
        {
            content.Add(new StringContent(dto.ModuleId.Value.ToString()), "ModuleId");
        }

        if (dto.ActivityId.HasValue)
        {
            content.Add(new StringContent(dto.ActivityId.Value.ToString()), "ActivityId");
        }

        if (dto.SubmissionId.HasValue)
        {
            content.Add(new StringContent(dto.SubmissionId.Value.ToString()), "SubmissionId");
        }

        if (document is not null)
        {
            document.AddToMultipart(content);
        }

        return content;
    }
}