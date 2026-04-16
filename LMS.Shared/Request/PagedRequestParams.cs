using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.Request;

public abstract class PagedRequestParams
{

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(2, 100)]
    public int PageSize { get; set; } = 10;

}

public class UsersRequestParams : PagedRequestParams { }

public class CoursesRequestParams : PagedRequestParams { }

public class ActivitiesRequestParams : PagedRequestParams
{
    public int? ModuleId { get; set; } = null;
}

public class SubmissionsRequestParams : PagedRequestParams
{
    public int? ActivityId { get; set; } = null;
    public string? StudentId { get; set; } = null;
}

public class ModulesRequestParams : PagedRequestParams
{
    public int? CourseId { get; set; } = null;
}

public class DocumentsRequestParams : PagedRequestParams
{
    public int? CourseId { get; set; } = null;
}


