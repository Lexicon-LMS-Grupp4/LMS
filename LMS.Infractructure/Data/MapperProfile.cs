using AutoMapper;
using Domain.Models.Entities;
using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.ActivityType;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Document;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.Submission;
using LMS.Shared.DTOs.SubmissionComment;
using LMS.Shared.DTOs.User;

namespace LMS.Infractructure.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // user mappings
        CreateMap<UserRegistrationDto, ApplicationUser>();

        CreateMap<CreateUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));

        CreateMap<ApplicationUser, UserBasicDto>();

        CreateMap<ApplicationUser, StudentBasicDto>()
            .IncludeBase<ApplicationUser, UserBasicDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null));

        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null));

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => (src.Students != null) ? src.Students.Count : 0))
            .ForMember(dest => dest.ModuleCount, opt => opt.MapFrom(src => src.Modules.Count))
            .ForMember(dest => dest.TeacherIds, opt => opt.MapFrom(src =>
                (src.CourseTeachers != null) ? src.CourseTeachers.Select(ct => ct.TeacherId).ToList() : new List<string>()))
            .ForMember(dest => dest.CurrentUserHasAccess, opt => opt.MapFrom((src, dest, _, context) =>
            {
                if (!context.TryGetItems(out var items))
                    return false;

                if (!items.TryGetValue("UserId", out var userIdObj))
                    return false;

                var userId = userIdObj as string;

                if (string.IsNullOrEmpty(userId))
                    return false;

                var isTeacher = src.CourseTeachers?.Any(ct => ct.TeacherId == userId) ?? false;
                var isStudent = src.Students?.Any(s => s.Id == userId) ?? false;

                return isTeacher || isStudent;
            }));


        CreateMap<Course, CourseDetailDto>()
            .IncludeBase<Course, CourseDto>()
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));

        CreateMap<CreateCourseDto, Course>();

        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Module mapping
        CreateMap<Module, ModuleDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : string.Empty))
            .ForMember(dest => dest.ActivityCount, opt => opt.MapFrom(src => src.Activities.Count));

        CreateMap<Module, ModuleDetailDto>();

        CreateMap<CreateModuleDto, Module>();

        // activity mappings
        // CreateMap<Activity, ActivityDto>()
        //    .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Documents.Count));
        //.ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src => src.Submissions.Count));

        // document mappings
        CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.Scope, opt => opt.MapFrom(
                src =>
                src.CourseId != null ? nameof(Course) :
                src.ModuleId != null ? nameof(Module) :
                src.ActivityId != null ? nameof(Activity) :
                src.SubmissionId != null ? nameof(Submission) :
                "Unknown"))
            .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => $"/api/documents/{src.Id}/file"))
            .ForMember(dest => dest.UploadedByUserName, opt => opt.MapFrom(src => src.UploadedByUser!.UserName))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : null))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module != null ? src.Module.Name : null))
            .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity != null ? src.Activity.Name : null))
            // quick fix to populate parent IDs for easier handling on frontend, since documents can be attached to any level and we want to easily determine the context:
            .ForMember(dest => dest.ParentCourseId, opt => opt.MapFrom(src =>
                src.CourseId ??
                (src.Module != null ? src.Module.CourseId :
                src.Activity != null ? src.Activity.Module!.CourseId :
                src.Submission != null ? src.Submission.Activity!.Module!.CourseId :
                null)
            ))
            .ForMember(dest => dest.ParentModuleId, opt => opt.MapFrom(src =>
                src.ModuleId ??
                (src.Activity != null ? src.Activity.ModuleId :
                src.Submission != null ? src.Submission.Activity!.ModuleId :
                null)
            ))
            .ForMember(dest => dest.ParentActivityId, opt => opt.MapFrom(src =>
                src.ActivityId ??
                (src.Submission != null ? src.Submission.ActivityId :
                null)
            ));

        CreateMap<CreateDocumentDto, Document>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.File.FileName))
            .ForMember(dest => dest.StoredFileName, opt => opt.Ignore())
            .ForMember(dest => dest.FileSize, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedByUser, opt => opt.Ignore());

        // ActivityType mappings
        CreateMap<ActivityType, ActivityTypeDto>();
        CreateMap<CreateActivityTypeDto, ActivityType>();
        CreateMap<UpdateActivityTypeDto, ActivityType>();

        // Activity mappings
        CreateMap<CreateActivityDto, Activity>();

        CreateMap<UpdateActivityDto, Activity>();

        CreateMap<PatchActivityDto, Activity>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Activity, ActivityDto>()
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src =>
                src.Module != null ? src.Module.Name : string.Empty))
            .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src =>
                src.ActivityTypeId))
            .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src =>
                src.ActivityType != null ? src.ActivityType.Name : string.Empty))
            .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src =>
                src.Documents.Count))
            .ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src =>
                src.Submissions
                    .Select(s => s.StudentId)
                    .Distinct()
                    .Count()))
            .IncludeAllDerived();

        CreateMap<Activity, ActivityDetailDto>();

        // submission mappings
        CreateMap<Submission, SubmissionDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.UserName))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.ActivityName, opt => opt.MapFrom(src => src.Activity.Name));

        CreateMap<Submission, SubmissionDetailDto>()
            .IncludeBase<Submission, SubmissionDto>();

        CreateMap<CreateSubmissionDto, Submission>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.SubmittedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsLate, opt => opt.Ignore());

        CreateMap<SubmissionComment, SubmissionCommentDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName));

    }
}
