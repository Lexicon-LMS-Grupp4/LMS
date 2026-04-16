using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Common;

namespace LMS.Blazor.Client.Services;

public class ActivityService : IActivityService
{
    private readonly IApiService _apiService;
    private const string Base = "api/activities";

    public ActivityService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<PagedResultDto<ActivityDto>?> GetAllActivitiesAsync(
        int page = 1,
        int pageSize = 10,
        int? moduleId = null,
        CancellationToken ct = default)
    {
        var query = $"{Base}?page={page}&pageSize={pageSize}";

        if (moduleId.HasValue)
            query += $"&moduleId={moduleId.Value}";

        return _apiService.GetAsync<PagedResultDto<ActivityDto>>(query, ct);
    }

    public Task<ActivityDetailDto?> GetActivityByIdAsync(int id, CancellationToken ct = default)
        => _apiService.GetAsync<ActivityDetailDto>($"{Base}/{id}/detail", ct);

    public Task<ActivityDetailDto?> GetActivityDetailByIdAsync(int id, CancellationToken ct = default)
        => _apiService.GetAsync<ActivityDetailDto>($"{Base}/{id}/detail", ct);

    public Task<ActivityDto?> CreateActivityAsync(CreateActivityDto dto, CancellationToken ct = default)
        => _apiService.PostAsync<ActivityDto>(Base, dto, ct);

    public Task DeleteActivityAsync(int id, CancellationToken ct = default)
        => _apiService.DeleteAsync($"{Base}/{id}", ct);

    public Task UpdateActivityAsync(int id, UpdateActivityDto dto, CancellationToken ct = default)
    => _apiService.PutAsync<object>($"{Base}/{id}", dto, ct);
    //public async Task DeleteActivityAsync(int id, CancellationToken ct = default)
    //{
    //    await _apiService.DeleteAsync($"{Base}/{id}", ct);
    //}

    public async Task<PagedResultDto<ActivityDto>> GetAllActivitiesByModuleAsync(
        int moduleId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var pagedResult = await _apiService.GetAsync<PagedResultDto<ActivityDto>>(
            $"{Base}?page={page}&pageSize={pageSize}&moduleId={moduleId}", ct);

        return pagedResult!;
    }
}