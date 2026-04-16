using LMS.Shared.DTOs.ActivityType;

namespace LMS.Blazor.Client.Services;

public class ActivityTypeService(IApiService apiService) : IActivityTypeService
{
    private readonly IApiService _apiService = apiService;
    private const string Base = "api/activity-types";

    public Task<IEnumerable<ActivityTypeDto>?> GetAllActivityTypesAsync(CancellationToken ct = default)
        => _apiService.GetAsync<IEnumerable<ActivityTypeDto>>(Base, ct);

    public Task<ActivityTypeDto?> GetActivityTypeByIdAsync(int id, CancellationToken ct = default)
        => _apiService.GetAsync<ActivityTypeDto>($"{Base}/{id}", ct);

    public Task<ActivityTypeDto?> CreateActivityTypeAsync(CreateActivityTypeDto dto, CancellationToken ct = default)
        => _apiService.PostAsync<ActivityTypeDto>(Base, dto, ct);

    public async Task UpdateActivityTypeAsync(int id, UpdateActivityTypeDto dto, CancellationToken ct = default)
        => await _apiService.PutAsync<object>($"{Base}/{id}", dto, ct);

    public async Task DeleteActivityTypeAsync(int id, CancellationToken ct = default)
        => await _apiService.DeleteAsync($"{Base}/{id}", ct);
}