using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Services.Access;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Common;
using LMS.Shared.Request;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services
{
    /// <summary>
    /// Activity service implementation
    /// </summary>
    public class ActivityService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILmsAccessService lmsAccessService,
        IUserAccessContextFactory userAccessContextFactory,
        IDocumentManager documentManager,
        UserManager<ApplicationUser> userManager) : IActivityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        //private readonly IDocumentManager _documentManager = documentManager;
        //private readonly ILmsAccessService _lmsAccessService = lmsAccessService;
        private readonly IUserAccessContextFactory _userAccessContextFactory = userAccessContextFactory;

        public async Task<PagedResultDto<ActivityDto>> GetAllActivitiesAsync(string userId, ActivitiesRequestParams queryDto)
        {
            var access = await _userAccessContextFactory.CreateAsync(userId);

            if (queryDto.ModuleId != null)
            {
                var module = await _unitOfWork.Modules.GetModuleAsync(queryDto.ModuleId.Value) ??
                    throw new NotFoundException($"Module by id: '{queryDto.ModuleId.Value}', does not exist");
                await lmsAccessService.EnsureCanAccessModuleAsync(userId, module);
            }

            var query = lmsAccessService.ApplyActivityAccessFilter(
                _unitOfWork.Activities.BuildQuery(queryDto.ModuleId),
                access);

            var (activities, totalCount) = await query
                .OrderBy(a => a.StartTime)
                .PagedResult(queryDto);

            return new PagedResultDto<ActivityDto>
            {
                Items = _mapper.Map<List<ActivityDto>>(activities),
                TotalCount = totalCount,
                PageNumber = queryDto.Page,
                PageSize = queryDto.PageSize
            };
        }

        public async Task<ActivityDto> GetActivityByIdAsync(int id, string userId)
        {
            var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(id) ??
                throw new NotFoundException($"Activity by id: '{id}', does not exist");

            await lmsAccessService.EnsureCanAccessActivityAsync(userId, activity);

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<ActivityDetailDto> GetActivityDetailByIdAsync(int id, string userId)
        {
            var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(id) ??
               throw new NotFoundException($"Activity by id: '{id}', does not exist");

            await lmsAccessService.EnsureCanAccessActivityAsync(userId, activity);

            var access = await _userAccessContextFactory.CreateAsync(userId);
            if (access.IsStudent && !access.IsTeacher && !access.IsAdmin)
            {
                activity.Submissions = lmsAccessService
                    .ApplySubmissionAccessFilter(activity.Submissions.AsQueryable(), access)
                    .OrderByDescending(s => s.SubmittedAt)
                    .ToList();
            }
            else
            {
                activity.Submissions = activity.Submissions
                    .GroupBy(s => s.StudentId)
                    .Select(g => g
                        .OrderByDescending(s => s.SubmittedAt)
                        .ThenByDescending(s => s.Id)
                        .First())
                    .OrderByDescending(s => s.SubmittedAt)
                    .ToList();
            }

            return _mapper.Map<ActivityDetailDto>(activity);
        }

        public async Task<ActivityDto> CreateActivityAsync(string userId, CreateActivityDto dto)
        {
            var module = await _unitOfWork.Modules.GetModuleAsync(dto.ModuleId) ??
                throw new NotFoundException($"Module by id: '{dto.ModuleId}', does not exist");

            var activityType = await _unitOfWork.ActivityTypes.FindByIdAsync(dto.ActivityTypeId) ??
                throw new NotFoundException($"ActivityType with id: '{dto.ActivityTypeId}' does not exist");

            //await _lmsAccessService.EnsureTeacherForCourseAsync(userId, module.CourseId);

            var activity = _mapper.Map<Activity>(dto);
            _unitOfWork.Activities.Create(activity);
            await _unitOfWork.CompleteAsync();

            // Assign nav properties AFTER save, purely for DTO mapping
            activity.Module = module;
            activity.ActivityType = activityType;

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task UpdateActivityAsync(int id, string userId, UpdateActivityDto dto)
        {
            var activity = await GetTeacherAccessedActivity(id, userId);

            _mapper.Map(dto, activity);

            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task PatchActivityAsync(int id, string userId, PatchActivityDto dto)
        {
            var activity = await GetTeacherAccessedActivity(id, userId);

            _mapper.Map(dto, activity);

            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteActivityAsync(int id, string userId)
        {
            var activity = await GetTeacherAccessedActivity(id, userId);

            if (activity.Documents.Any())
            {
                await documentManager.DeleteManyAsync(activity.Documents);
            }

            _unitOfWork.Activities.Delete(activity);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<Activity> GetTeacherAccessedActivity(int id, string userId)
        {
            var activity = await _unitOfWork.Activities.GetActivityWithRelationsAsync(id) ??
                throw new NotFoundException($"Activity by id: '{id}', does not exist");

            //await  _lmsAccessService.EnsureTeacherForCourseAsync(userId, activity.Module.CourseId);
            return activity;
        }
    }
}
