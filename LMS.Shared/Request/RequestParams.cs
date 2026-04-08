using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.Request;

public abstract class RequestParams
{

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(2, 100)]
    public int PageSize { get; set; } = 10;

}

public class UsersRequestParams : RequestParams { }

public class CoursesRequestParams : RequestParams { }

public class ActivitiesRequestParams : RequestParams
{
    public int? ModuleId { get; set; } = null;
}

public class SubmissionsRequestParams : RequestParams
{
    public int? ActivityId { get; set; } = null;
    public int? StudentId { get; set; } = null;
}

public class ModulesRequestParams : RequestParams
{
    public int? CourseId { get; set; } = null;
}

public class DocumentsRequestParams : RequestParams
{
    public int? CourseId { get; set; } = null;
}


