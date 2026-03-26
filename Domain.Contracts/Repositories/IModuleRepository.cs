using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>
{
    Task<(IEnumerable<Module> Modules, int TotalCount)> GetAllModulesAsync(
        int page, int pageSize, int? courseId, bool trackChanges = false);
    Task<Module?> GetModuleAsync(int id, bool trackChanges = false);

    Task<Module?> GetModuleByNameAsync(int courseId, string name, bool trackChanges = false);
}