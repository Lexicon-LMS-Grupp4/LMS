using Domain.Models.Entities;
using LMS.Shared.DTOs.Document;

namespace Domain.Contracts.Repositories;

public interface IDocumentRepository : IRepositoryBase<Document>, IInternalRepositoryBase<Document>
{
    Task<(IEnumerable<Document> documents, int totalCount)> GetDocumentsAsync(int page, int pageSize, DocumentQueryDto dto);
    Task<Document?> GetDocumentAsync(int id, bool trackChanges = false);
    Task<Document?> GetDocumentWithAccessRelationsAsync(int id, bool trackChanges = false);
    IQueryable<Document> BuildBasicQuery(bool trackChanges = false);
    IQueryable<Document> BuildBasicQuery(DocumentQueryDto dto, bool trackChanges = false);
    IQueryable<Document> BuildAccessRelationsQuery(bool trackChanges = false);
    IQueryable<Document> BuildAccessRelationsQuery(DocumentQueryDto dto, bool trackChanges = false);
}