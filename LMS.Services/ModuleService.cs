using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Infractructure.Extensions;
using LMS.Services.Access;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;
using Service.Contracts;

namespace LMS.Services;

/// <summary>
/// Module service implementation
/// </summary>
public class ModuleService : IModuleService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly ILmsAccessService lmsAccessService;
    private readonly IUserAccessContextFactory userAccessContextFactory;
    private readonly IDocumentManager documentManager;
    public ModuleService(IUnitOfWork unitOfWork,
                         IMapper mapper,
                         ILmsAccessService lmsAccessService,
                         IDocumentManager documentManager,
                         IUserAccessContextFactory userAccessContextFactor)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.lmsAccessService = lmsAccessService;
        this.userAccessContextFactory = userAccessContextFactor;
        this.documentManager = documentManager;
    }
    public async Task<PagedResultDto<ModuleDto>> GetAllModulesAsync(string userId, int page, int pageSize, int? courseId = null)
    {
        if (courseId != null)
        {
            await lmsAccessService.EnsureCanAccessCourseAsync(userId, courseId.Value);
        }

        var access = await userAccessContextFactory.CreateAsync(userId);

        var query = lmsAccessService.ApplyModuleAccessFilter(
            unitOfWork.Modules.BuildQuery(courseId),
            access);

        var (modules, totalCount) = await query
            .OrderBy(m => m.StartDate)
            .PagedResult(page, pageSize);

        return new PagedResultDto<ModuleDto>
        {
            Items = mapper.Map<List<ModuleDto>>(modules),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };

    }

    public async Task<ModuleDto> GetModuleByIdAsync(int id, string userId)
    {

        var module = await unitOfWork.Modules.GetModuleAsync(id)
            ?? throw new NotFoundException($"Module with id {id} not found");

        await lmsAccessService.EnsureCanAccessModuleAsync(userId, module);

        return mapper.Map<ModuleDto>(module);
    }

    public async Task<ModuleDetailDto> GetModuleDetailByIdAsync(int id, string userId)
    {
        var module = await unitOfWork.Modules.GetModuleAsync(id)
            ?? throw new NotFoundException($"Module with id {id} not found");

        await lmsAccessService.EnsureCanAccessModuleAsync(userId, module);

        return mapper.Map<ModuleDetailDto>(module);
    }

    public async Task<ModuleDto> CreateModuleAsync(string userId, CreateModuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await lmsAccessService.EnsureTeacherForCourseAsync(userId, dto.CourseId);

        await ThrowIfNotValidModule(dto.CourseId, 0, dto.StartDate, dto.EndDate, dto.Name);

        var module = mapper.Map<Module>(dto);
        unitOfWork.Modules.Create(module);
        await unitOfWork.CompleteAsync();

        // refetch new module to verify it exists
        var createdModule = await unitOfWork.Modules.GetModuleAsync(module.Id)
            ?? throw new InvalidOperationException($"Module with id {module.Id} could not be loaded after creation.");

        return mapper.Map<ModuleDto>(createdModule);

    }

    private async Task ThrowIfNotValidModule(Module module)
    {
        await ThrowIfNotValidModule(module.CourseId, module.Id, module.StartDate, module.EndDate, module.Name);
    }

    private async Task ThrowIfNotValidModule(int courseId,
                                             int moduleId,
                                             DateTime moduleStartDate,
                                             DateTime moduleEndDate,
                                             string name)
    {
        var course = await unitOfWork.Courses.FindByIdOrThrowAsync(courseId, trackChanges: true);

        if (course.StartDate > moduleStartDate)
        {
            throw new BadRequestException("Module start date cannot be earlier than course start date.");
        }
        if (moduleStartDate >= moduleEndDate)
        {
            throw new BadRequestException("End date must be greater than start date.");
        }

        var existingModule = await unitOfWork.Modules.GetModuleByNameAsync(courseId, name);
        if (existingModule is not null && existingModule.Id != moduleId)
        {
            throw new BadRequestException("Module name must be unique within course");
        }
        // TODO: add validation for...
        // 1 not overlapping modules
        // 2 setting start and end times to reasonable values (08:00 - 17:00?)
        // 3 checking for weekends? assignment tasks within a module could be set on sundays, but maybe a module never starts on a weekend

    }

    public async Task UpdateModuleAsync(int id, string userId, UpdateModuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var module = await unitOfWork.Modules.FindByIdOrThrowAsync(id, trackChanges: true);

        await lmsAccessService.EnsureTeacherForCourseAsync(userId, module.CourseId);

        module.Name = dto.Name;
        module.Description = dto.Description;
        module.StartDate = dto.StartDate;
        module.EndDate = dto.EndDate;
        module.Icon = dto.Icon;

        await ThrowIfNotValidModule(module);

        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateModulePartiallyAsync(int id, string userId, PatchModuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var module = await unitOfWork.Modules.FindByIdOrThrowAsync(id, trackChanges: true);

        await lmsAccessService.EnsureTeacherForCourseAsync(userId, module.CourseId);

        module.Name = string.IsNullOrEmpty(dto.Name) ? module.Name : dto.Name;
        if (dto.Description is not null)
        {
            module.Description = dto.Description;
        }
        module.StartDate = dto.StartDate ?? module.StartDate;
        module.EndDate = dto.EndDate ?? module.EndDate;
        if (dto.Icon is not null)
        {
            module.Icon = dto.Icon;
        }

        await ThrowIfNotValidModule(module.CourseId, module.Id, module.StartDate, module.EndDate, module.Name);

        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteModuleAsync(int id, string userId)
    {
        var module = await unitOfWork.Modules.GetModuleAsync(id, trackChanges: true)
            ?? throw new NotFoundException($"Module with id {id} does not exist.");

        await lmsAccessService.EnsureTeacherForCourseAsync(userId, module.CourseId);

        if (module.Documents.Any())
        {
            await documentManager.DeleteManyAsync(module.Documents);
        }

        unitOfWork.Modules.Delete(module);
        await unitOfWork.CompleteAsync();
    }
}
