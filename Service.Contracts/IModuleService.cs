using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Module operations
    /// </summary>
    public interface IModuleService
    {
        Task<PagedResultDto<ModuleDto>> GetAllModulesAsync(string userId, int page, int pageSize, int? courseId = null);
        Task<ModuleDto> GetModuleByIdAsync(int id, string userId);
        Task<ModuleDetailDto> GetModuleDetailByIdAsync(int id, string userId);
        Task<ModuleDto> CreateModuleAsync(string userId, CreateModuleDto dto);
        Task UpdateModuleAsync(int id, string userId, UpdateModuleDto dto);
        Task UpdateModulePartiallyAsync(int id, string userId, PatchModuleDto dto);
        Task DeleteModuleAsync(int id, string userId);
    }
}
