using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Submission operations
    /// </summary>
    public interface ISubmissionService
    {
        Task<PagedResultDto<SubmissionDto>> GetAllSubmissionsAsync(int page, int pageSize, int? activityId = null, string? studentId = null);
        Task<SubmissionDto?> GetSubmissionByIdAsync(int id);
        Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto);
        Task UpdateSubmissionAsync(int id, UpdateSubmissionDto dto);
        Task DeleteSubmissionAsync(int id);
        Task SubmitFeedbackAsync(int id, SubmitFeedbackDto dto);
    }
}
