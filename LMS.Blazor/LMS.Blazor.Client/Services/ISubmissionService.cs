
using global::LMS.Blazor.Client.Uploads;
using global::LMS.Shared.DTOs.Submission;
using LMS.Shared.DTOs.Common;

namespace LMS.Blazor.Client.Services;

public interface ISubmissionService
{
    Task<PagedResultDto<SubmissionDto>?> GetAllSubmissionsAsync(
        int page = 1,
        int pageSize = 10,
        int? activityId = null,
        string? studentId = null,
        CancellationToken ct = default);

    Task<SubmissionDto?> GetSubmissionByIdAsync(int id, CancellationToken ct = default);

    Task<SubmissionDto?> CreateSubmissionAsync(
        CreateSubmissionDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default);

    Task<SubmissionDto?> UpdateSubmissionAsync(
        int submissionId,
        UpdateSubmissionDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default);

    Task DeleteSubmissionAsync(int id, CancellationToken ct = default);

    Task<SubmissionDetailDto?> GetSubmissionDetailByIdAsync(int id, CancellationToken ct = default);
    Task SubmitCommentAsync(int submissionId, SubmitCommentDto dto, CancellationToken ct = default);
}