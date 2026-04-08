using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;

namespace LMS.Blazor.Services;

public class ServerNoOpModuleService(ILogger<ServerNoOpModuleService> logger) : IModuleService
{
    private readonly ILogger<ServerNoOpModuleService> _logger = logger;

    public Task<PagedResultDto<ModuleDto>?> GetAllModulesAsync(int page = 1, int pageSize = 10, int? courseId = null, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for GetAllModulesAsync");
        return Task.FromResult<PagedResultDto<ModuleDto>?>(null);
    }

    public Task<ModuleDto?> GetModuleByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for GetModuleByIdAsync: {ModuleId}", id);
        return Task.FromResult<ModuleDto?>(null);
    }

    public Task<ModuleDetailDto?> GetModuleDetailByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for GetModuleDetailByIdAsync: {ModuleId}", id);
        return Task.FromResult<ModuleDetailDto?>(null);
    }

    public Task<ModuleDto?> CreateModuleAsync(CreateModuleDto dto, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for CreateModuleAsync");
        return Task.FromResult<ModuleDto?>(null);
    }

    public Task UpdateModuleAsync(int id, UpdateModuleDto dto, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for UpdateModuleAsync: {ModuleId}", id);
        return Task.CompletedTask;
    }

    public Task DeleteModuleAsync(int id, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpModuleService called for DeleteModuleAsync: {ModuleId}", id);
        return Task.CompletedTask;
    }
}
