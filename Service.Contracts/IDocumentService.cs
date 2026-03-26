using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Document operations
    /// </summary>
    public interface IDocumentService
    {
        Task<PagedResultDto<DocumentDto>> GetAllDocumentsAsync(int page, int pageSize, int? courseId = null);
        Task<DocumentDto?> GetDocumentByIdAsync(int id);
        Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto);
        Task UpdateDocumentAsync(int id, UpdateDocumentDto dto);
        Task DeleteDocumentAsync(int id);
    }
}
