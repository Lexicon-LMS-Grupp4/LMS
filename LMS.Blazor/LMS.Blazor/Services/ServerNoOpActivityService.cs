using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Common;

namespace LMS.Blazor.Services;

public class ServerNoOpActivityService : IActivityService
{
    public Task<PagedResultDto<ActivityDto>?> GetAllActivitiesAsync(int page = 1, int pageSize = 10, int? moduleId = null, CancellationToken ct = default)
        => Task.FromResult<PagedResultDto<ActivityDto>?>(null);

    public Task<ActivityDto?> GetActivityByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult<ActivityDto?>(null);

    public Task<ActivityDto?> CreateActivityAsync(CreateActivityDto dto, CancellationToken ct = default)
        => Task.FromResult<ActivityDto?>(null);

    public Task DeleteActivityAsync(int id, CancellationToken ct = default)
        => Task.CompletedTask;
}