using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IDocumentRepository : IRepositoryBase<Document>, IInternalRepositoryBase<Document>
{

}