using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;
using LMS.Shared.Request;

namespace Service.Contracts
{
    /// <summary>
    /// Service contract for Submission operations
    /// </summary>
    public interface ISubmissionService
    {
        Task<PagedResultDto<SubmissionDto>> GetAllSubmissionsAsync(string userId, SubmissionsRequestParams query);
        Task<SubmissionDto> GetSubmissionByIdAsync(int id, string userId);
        Task<SubmissionDetailDto> GetSubmissionDetailByIdAsync(int id, string userId);
        Task<SubmissionDto> CreateSubmissionAsync(string userId, CreateSubmissionDto dto);
        Task UpdateSubmissionAsync(int id, string userId, UpdateSubmissionDto dto);
        Task UpdateSubmissionPartiallyAsync(int id, string userId, PatchSubmissionDto dto);
        Task DeleteSubmissionAsync(int id, string userId);
        Task SubmitCommentAsync(int id, string userId, SubmitCommentDto dto);
    }
}
