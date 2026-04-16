using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Contracts.Storage;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Services.Access;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly ILmsAccessService lmsAccessService;
    private readonly IUserAccessContextFactory userAccessContextFactory;
    private readonly IDocumentManager documentManager;
    public CourseService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILmsAccessService lmsAccessService,
        IDocumentManager documentManager,
        IUserAccessContextFactory userAccessContextFactory)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.lmsAccessService = lmsAccessService;
        this.userAccessContextFactory = userAccessContextFactory;
        this.documentManager = documentManager;
    }

    public async Task<PagedResultDto<CourseDto>> GetAllCoursesAsync(string userId, int page, int pageSize)
    {
        //let all courses be visible to all authenticated users
        //var access = await userAccessContextFactory.CreateAsync(userId);

        //var query = unitOfWork.Courses.BuildQuery();

        //query = lmsAccessService.ApplyCourseAccessFilter(query, access);

        var (courses, totalCount) = await unitOfWork.Courses.GetAllCoursesAsync(page, pageSize);

        return new PagedResultDto<CourseDto>
        {
            Items = mapper.Map<List<CourseDto>>(courses, opt =>
            {
                opt.Items["UserId"] = userId;
            }),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<CourseDto> GetCourseByIdAsync(int id, string userId)
    {
        //let single courses be visible to all authenticated users
        var course = await unitOfWork.Courses.GetCourseAsync(id)
            ?? throw new NotFoundException($"Course with id {id} not found");

        return mapper.Map<CourseDto>(course, opt =>
        {
            opt.Items["UserId"] = userId;
        });
    }


    public async Task<CourseDetailDto> GetCourseDetailByIdAsync(int id, string userId)
    {
        var course = await unitOfWork.Courses.GetCourseAsync(id)
            ?? throw new NotFoundException($"Course with id {id} not found");

        await lmsAccessService.EnsureCanAccessCourseAsync(userId, course);

        return mapper.Map<CourseDetailDto>(course, opt =>
        {
            opt.Items["UserId"] = userId;
        });
    }

    public async Task<CourseDto> CreateCourseAsync(string userId, CreateCourseDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var access = await userAccessContextFactory.CreateAsync(userId);
        if (!access.IsTeacher)
        {
            throw new UnauthorizedAccessException("Only teachers can create courses.");
        }

        var course = mapper.Map<Course>(dto);

        course.CourseTeachers = new List<CourseTeacher>
        {
            new CourseTeacher
            {
                TeacherId = userId
            }
        };

        unitOfWork.Courses.Create(course);
        await unitOfWork.CompleteAsync();

        return mapper.Map<CourseDto>(course, opt =>
        {
            opt.Items["UserId"] = userId;
        });
    }

    public async Task UpdateCourseAsync(int id, string userId, UpdateCourseDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: true)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        //await lmsAccessService.EnsureTeacherForCourseAsync(userId, course);

        mapper.Map(dto, course);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteCourseAsync(int id, string userId)
    {
        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: true)
            ?? throw new NotFoundException($"Course with id {id} was not found.");

        //await lmsAccessService.EnsureTeacherForCourseAsync(userId, course);

        if (course.Documents.Any())
        {
            await documentManager.DeleteManyAsync(course.Documents);
        }

        unitOfWork.Courses.Delete(course);
        await unitOfWork.CompleteAsync();
    }
}