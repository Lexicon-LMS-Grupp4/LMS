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

    public Task<ActivityDto?> GetActivityByIdAsync(int id, CancellationToken ct = default)
        => _apiService.GetAsync<ActivityDto>($"{Base}/{id}", ct);

    public Task<ActivityDto?> CreateActivityAsync(CreateActivityDto dto, CancellationToken ct = default)
        => _apiService.PostAsync<ActivityDto>(Base, dto, ct);

    public async Task DeleteActivityAsync(int id, CancellationToken ct = default)
    {
        await _apiService.DeleteAsync($"{Base}/{id}", ct);
    }
}