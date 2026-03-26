using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IModuleService> moduleService;
    private Lazy<IActivityService> activityService;
    private Lazy<IDocumentService> documentService;
    private Lazy<ISubmissionService> submissionService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IModuleService ModuleService => moduleService.Value;
    public IActivityService ActivityService => activityService.Value;
    public IDocumentService DocumentService => documentService.Value;
    public ISubmissionService SubmissionService => submissionService.Value;


    public ServiceManager(Lazy<IAuthService> authService,        
        Lazy<ICourseService> courseService,
        Lazy<IModuleService> moduleService,
        Lazy<IActivityService> activityService,
        Lazy<IDocumentService> documentService,
        Lazy<ISubmissionService> submissionService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.moduleService = moduleService;
        this.activityService = activityService;
        this.documentService = documentService;
        this.submissionService = submissionService;

    }
}
