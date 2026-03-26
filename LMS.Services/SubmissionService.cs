using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Submission;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    /// <summary>
    /// Submission service implementation - TODO: Replace with real database operations
    /// </summary>
    public class SubmissionService : ISubmissionService
    {
        public async Task<PagedResultDto<SubmissionDto>> GetAllSubmissionsAsync(int page, int pageSize, int? activityId = null, string? studentId = null)
        {
            // TODO: Replace with real database query
            var mockSubmissions = new List<SubmissionDto>
        {
            new SubmissionDto
            {
                Id = 1,
                StudentId = studentId ?? "student-1",
                StudentName = "John Doe",
                ActivityId = activityId ?? 1,
                ActivityName = "Variables and Data Types",
                Body = "My submission for this assignment...",
                DocumentId = null,
                Document = null,
                SubmittedAt = DateTime.Now,
                IsLate = false,
                FeedbackText = null,
                FeedbackGivenAt = null
            }
        };

            return await Task.FromResult(new PagedResultDto<SubmissionDto>
            {
                Items = mockSubmissions,
                TotalCount = mockSubmissions.Count,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        public async Task<SubmissionDto?> GetSubmissionByIdAsync(int id)
        {
            // TODO: Replace with real database query
            return await Task.FromResult(new SubmissionDto
            {
                Id = id,
                StudentId = "student-1",
                StudentName = "John Doe",
                ActivityId = 1,
                ActivityName = "Variables and Data Types",
                Body = "My submission for this assignment...",
                DocumentId = null,
                Document = null,
                SubmittedAt = DateTime.Now,
                IsLate = false,
                FeedbackText = null,
                FeedbackGivenAt = null
            });
        }

        public async Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto)
        {
            // TODO: Create entity and save to database
            return await Task.FromResult(new SubmissionDto
            {
                Id = 1,
                StudentId = dto.StudentId,
                StudentName = "Student Name",
                ActivityId = dto.ActivityId,
                ActivityName = "Activity Name",
                Body = dto.Body,
                DocumentId = dto.DocumentId,
                Document = null,
                SubmittedAt = DateTime.Now,
                IsLate = false,
                FeedbackText = null,
                FeedbackGivenAt = null
            });
        }

        public async Task UpdateSubmissionAsync(int id, UpdateSubmissionDto dto)
        {
            // TODO: Update entity in database
            await Task.CompletedTask;
        }

        public async Task DeleteSubmissionAsync(int id)
        {
            // TODO: Delete entity from database
            await Task.CompletedTask;
        }

        public async Task SubmitFeedbackAsync(int id, SubmitFeedbackDto dto)
        {
            // TODO: Update submission with feedback
            await Task.CompletedTask;
        }
    }
}
