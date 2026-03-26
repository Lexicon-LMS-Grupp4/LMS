using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;

public class DocumentRepository(ApplicationDbContext context) : RepositoryBase<Document>(context), IDocumentRepository
{
}