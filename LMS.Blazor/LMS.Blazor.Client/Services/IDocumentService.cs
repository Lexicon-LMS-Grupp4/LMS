using LMS.Blazor.Client.Uploads;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Document;

namespace LMS.Blazor.Client.Services;

public interface IDocumentService
{
    Task<PagedResultDto<DocumentDto>?> GetAllDocumentsAsync(
        int page = 1,
        int pageSize = 10,
        DocumentQueryDto? query = null,
        CancellationToken ct = default);

    Task<DocumentDto?> GetDocumentByIdAsync(int id, CancellationToken ct = default);

    Task<DocumentDto?> CreateDocumentAsync(
        CreateDocumentDto dto,
        UploadDocumentValue? document = null,
        CancellationToken ct = default);

    Task<DocumentDto?> UpdateDocumentAsync(
        int id,
        UpdateDocumentDto dto,
        CancellationToken ct = default);

    Task DeleteDocumentAsync(int id, CancellationToken ct = default);

    string GetDocumentFileUrl(int id, bool download = false);
}