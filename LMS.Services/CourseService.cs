using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Course;
using Service.Contracts;

namespace LMS.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<PagedResultDto<CourseDto>> GetAllCoursesAsync(int page, int pageSize)
    {
        var (courses, totalCount) = await unitOfWork.Courses.GetAllCoursesAsync(page, pageSize);

        return new PagedResultDto<CourseDto>
        {
            Items = mapper.Map<List<CourseDto>>(courses),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: false);
        return course is null ? null : mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDetailDto?> GetCourseDetailByIdAsync(int id)
    {
        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: false);
        return course is null ? null : mapper.Map<CourseDetailDto>(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var course = mapper.Map<Course>(dto);
        unitOfWork.Courses.Create(course);
        await unitOfWork.CompleteAsync();

        return mapper.Map<CourseDto>(course);
    }

    public async Task UpdateCourseAsync(int id, UpdateCourseDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: true);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {id} was not found.");

        mapper.Map(dto, course);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteCourseAsync(int id)
    {
        var course = await unitOfWork.Courses.GetCourseAsync(id, trackChanges: true);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {id} was not found.");

        unitOfWork.Courses.Delete(course);
        await unitOfWork.CompleteAsync();
    }
}