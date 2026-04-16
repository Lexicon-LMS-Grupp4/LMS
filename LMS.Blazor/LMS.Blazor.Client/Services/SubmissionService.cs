using LMS.Blazor.Client.Uploads;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;

namespace LMS.Blazor.Client.Services;

public class SubmissionService : ISubmissionService
{
    private readonly IApiService _apiService;
    private readonly ILogger<SubmissionService> _logger;

    private const string Base = "api/submissions";

    public SubmissionService(IApiService apiService, ILogger<SubmissionService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResultDto<SubmissionDto>?> GetAllSubmissionsAsync(
        int page = 1,
        int pageSize = 10,
        int? activityId = null,
        string? studentId = null,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"{Base}?page={page}&pageSize={pageSize}";

            if (activityId.HasValue)
                url += $"&activityId={activityId.Value}";

            if (!string.IsNullOrWhiteSpace(studentId))
                url += $"&studentId={Uri.EscapeDataString(studentId)}";

            return await _apiService.GetAsync<PagedResultDto<SubmissionDto>>(url, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all submissions.");
            return null;
        }
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<SubmissionDto>($"{Base}/{id}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching submission {SubmissionId}.", id);
            return null;
        }
    }

    public async Task<SubmissionDto?> CreateSubmissionAsync(
        CreateSubmissionDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default)
    {
        try
        {
            using var content = BuildSubmissionContent(dto, document);
            return await _apiService.PostMultipartAsync<SubmissionDto>(Base, content, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission.");
            return null;
        }
    }
    public async Task<SubmissionDto?> UpdateSubmissionAsync(
        int submissionId,
        UpdateSubmissionDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default)
    {
        try
        {
            using var content = BuildSubmissionContent(dto, document);
            await _apiService.PutMultipartAsync<SubmissionDto>($"{Base}/{submissionId}", content, ct);
            return await GetSubmissionByIdAsync(submissionId, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission.");
            return null;
        }
    }

    public async Task DeleteSubmissionAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _apiService.DeleteAsync($"{Base}/{id}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting submission {SubmissionId}.", id);
        }
    }

    private static MultipartFormDataContent BuildSubmissionContent(
        CreateSubmissionDto dto,
        UploadDocumentValue? document)
    {
        var content = new MultipartFormDataContent();

        var activityIdContent = new StringContent(dto.ActivityId.ToString());
        content.Add(activityIdContent, "ActivityId");

        if (!string.IsNullOrWhiteSpace(dto.FileDescription))
        {
            var fileDescriptionContent = new StringContent(dto.FileDescription.ToString());
            content.Add(fileDescriptionContent, "FileDescription");
        }
        if (!string.IsNullOrWhiteSpace(dto.Body))
        {
            var bodyContent = new StringContent(dto.Body);
            content.Add(bodyContent, "Body");
        }

        if (dto.RemoveFile)
        {
            content.Add(new StringContent(bool.TrueString), "RemoveFile");
        }

        if (document is not null)
        {
            document.AddToMultipart(content);
        }

        return content;
    }

    public async Task<SubmissionDetailDto?> GetSubmissionDetailByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<SubmissionDetailDto>($"{Base}/{id}/detail", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching submission detail {SubmissionId}.", id);
            return null;
        }
    }

    public async Task SubmitCommentAsync(int submissionId, SubmitCommentDto dto, CancellationToken ct = default)
    {
        try
        {
            await _apiService.PostAsync<object>($"{Base}/{submissionId}/comment", dto, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting comment for submission {SubmissionId}.", submissionId);
            throw;
        }
    }
}
