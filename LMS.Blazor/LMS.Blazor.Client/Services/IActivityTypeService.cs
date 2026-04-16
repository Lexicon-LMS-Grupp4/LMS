using LMS.Shared.DTOs.ActivityType;

namespace LMS.Blazor.Client.Services;

public interface IActivityTypeService
{
    Task<IEnumerable<ActivityTypeDto>?> GetAllActivityTypesAsync(CancellationToken ct = default);
    Task<ActivityTypeDto?> GetActivityTypeByIdAsync(int id, CancellationToken ct = default);
    Task<ActivityTypeDto?> CreateActivityTypeAsync(CreateActivityTypeDto dto, CancellationToken ct = default);
    Task UpdateActivityTypeAsync(int id, UpdateActivityTypeDto dto, CancellationToken ct = default);
    Task DeleteActivityTypeAsync(int id, CancellationToken ct = default);
}