using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Common;

namespace LMS.Blazor.Client.Services;

public interface IActivityService
{
    Task<PagedResultDto<ActivityDto>?> GetAllActivitiesAsync(int page = 1, int pageSize = 10, int? moduleId = null, CancellationToken ct = default);
    Task<ActivityDetailDto?> GetActivityByIdAsync(int id, CancellationToken ct = default);
    Task<ActivityDetailDto?> GetActivityDetailByIdAsync(int id, CancellationToken ct = default);
    Task<ActivityDto?> CreateActivityAsync(CreateActivityDto dto, CancellationToken ct = default);
    Task UpdateActivityAsync(int id, UpdateActivityDto dto, CancellationToken ct = default);
    Task DeleteActivityAsync(int id, CancellationToken ct = default);
}