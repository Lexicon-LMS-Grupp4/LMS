using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.ActivityType;
using Service.Contracts;

namespace LMS.Services;

public class ActivityTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IActivityTypeService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ActivityTypeDto>> GetAllActivityTypesAsync()
    {
        var types = await _unitOfWork.ActivityTypes.GetAllActivityTypesAsync();
        return _mapper.Map<IEnumerable<ActivityTypeDto>>(types);
    }

    public async Task<ActivityTypeDto> GetActivityTypeByIdAsync(int id)
    {
        var type = await _unitOfWork.ActivityTypes.FindByIdAsync(id) ??
            throw new NotFoundException($"ActivityType with id: '{id}' does not exist");

        return _mapper.Map<ActivityTypeDto>(type);
    }

    public async Task<ActivityTypeDto> CreateActivityTypeAsync(CreateActivityTypeDto dto)
    {
        var existing = await _unitOfWork.ActivityTypes.GetActivityTypeByNameAsync(dto.Name);
        if (existing is not null)
            throw new BadRequestException($"ActivityType with name: '{dto.Name}' already exists");

        var activityType = _mapper.Map<ActivityType>(dto);

        _unitOfWork.ActivityTypes.Create(activityType);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ActivityTypeDto>(activityType);
    }

    public async Task UpdateActivityTypeAsync(int id, UpdateActivityTypeDto dto)
    {
        var activityType = await _unitOfWork.ActivityTypes.FindByIdAsync(id) ??
            throw new NotFoundException($"ActivityType with id: '{id}' does not exist");

        _mapper.Map(dto, activityType);

        _unitOfWork.ActivityTypes.Update(activityType);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteActivityTypeAsync(int id)
    {
        var activityType = await _unitOfWork.ActivityTypes.FindByIdAsync(id) ??
            throw new NotFoundException($"ActivityType with id: '{id}' does not exist");
        var isInUse = await _unitOfWork.Activities.AnyWithActivityTypeAsync(id);
        if (isInUse)
            throw new BadRequestException(
                $"Cannot delete '{activityType.Name}' — it is assigned to one or more activities.");
        _unitOfWork.ActivityTypes.Delete(activityType);
        await _unitOfWork.CompleteAsync();
    }
}