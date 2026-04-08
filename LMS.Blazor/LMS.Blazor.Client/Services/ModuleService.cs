using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ModuleService : IModuleService
{
    private readonly IApiService _apiService;
    private readonly ILogger<ModuleService> _logger;

    private const string Base = "api/modules";

    public ModuleService(IApiService apiService, ILogger<ModuleService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PagedResultDto<ModuleDto>?> GetAllModulesAsync(int page = 1, int pageSize = 10, int? courseId = null, CancellationToken ct = default)
    {
        try
        {
            var url = $"{Base}?page={page}&pageSize={pageSize}";
            if (courseId.HasValue) url += $"&courseId={courseId.Value}";

            return await _apiService.GetAsync<PagedResultDto<ModuleDto>>(url, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all modules.");
            return null;
        }
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<ModuleDto>($"{Base}/{id}", ct);
    }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching module {ModuleId}.", id);
            return null;
        }
    }

    public async Task<ModuleDetailDto?> GetModuleDetailByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.GetAsync<ModuleDetailDto>($"{Base}/{id}/detail", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching module detail {ModuleId}.", id);
            return null;
        }
    }

    public async Task<ModuleDto?> CreateModuleAsync(CreateModuleDto dto, CancellationToken ct = default)
    {
        try
        {
            return await _apiService.PostAsync<ModuleDto>(Base, dto, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating module.");
            return null;
        }
    }

    public async Task UpdateModuleAsync(int id, UpdateModuleDto dto, CancellationToken ct = default)
    {
        try
        {
            await _apiService.PutAsync<object>($"{Base}/{id}", dto, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating module {ModuleId}.", id);
        }
    }

    public async Task DeleteModuleAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _apiService.DeleteAsync($"{Base}/{id}", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting module {ModuleId}.", id);
        }
    }
}