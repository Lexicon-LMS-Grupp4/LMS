using LMS.Shared.DTOs.ActivityType;

namespace Service.Contracts;

public interface IActivityTypeService
{
    Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync();
    Task<ActivityTypeDto> GetActivityTypeByIdAsync(int id);
    Task<ActivityTypeDto> CreateActivityTypeAsync(CreateActivityTypeDto dto);
    Task UpdateActivityTypeAsync(int id, UpdateActivityTypeDto dto);
    Task DeleteActivityTypeAsync(int id);
}