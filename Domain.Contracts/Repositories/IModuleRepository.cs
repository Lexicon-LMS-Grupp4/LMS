using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>
{
    IQueryable<Module> BuildQuery(int? courseId = null, bool trackChanges = false);
    Task<Module?> GetModuleAsync(int id, bool trackChanges = false);
    Task<Module?> GetModuleByNameAsync(int courseId, string name, bool trackChanges = false);


}