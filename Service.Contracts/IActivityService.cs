using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Activity operations
    /// </summary>
    public interface IActivityService
    {
        Task<PagedResultDto<ActivityDto>> GetAllActivitiesAsync(int page, int pageSize, int? moduleId = null);
        Task<ActivityDto?> GetActivityByIdAsync(int id);
        Task<ActivityDetailDto?> GetActivityDetailByIdAsync(int id);
        Task<ActivityDto> CreateActivityAsync(CreateActivityDto dto);
        Task UpdateActivityAsync(int id, UpdateActivityDto dto);
        Task DeleteActivityAsync(int id);
    }
}
