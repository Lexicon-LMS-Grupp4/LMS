using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Module operations
    /// </summary>
    public interface IModuleService
    {
        Task<PagedResultDto<ModuleDto>> GetAllModulesAsync(int page, int pageSize, int? courseId = null);
        Task<ModuleDto?> GetModuleByIdAsync(int id);
        Task<ModuleDetailDto?> GetModuleDetailByIdAsync(int id);
        Task<ModuleDto> CreateModuleAsync(CreateModuleDto dto);
        Task UpdateModuleAsync(int id, UpdateModuleDto dto);
        Task UpdateModulePartiallyAsync(int id, PatchModuleDto dto);
        Task DeleteModuleAsync(int id);
    }
}
