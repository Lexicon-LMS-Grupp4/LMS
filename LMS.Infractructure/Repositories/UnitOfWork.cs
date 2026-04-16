using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<ICourseRepository> _courses;
    private readonly Lazy<IModuleRepository> _modules;
    private readonly Lazy<IActivityRepository> _activities;
    private readonly Lazy<IActivityTypeRepository> _activityTypes;
    private readonly Lazy<IDocumentRepository> _documents;
    private readonly Lazy<ISubmissionRepository> _submissions;

    public ICourseRepository Courses => _courses.Value;
    public IModuleRepository Modules => _modules.Value;
    public IActivityRepository Activities => _activities.Value;
    public IActivityTypeRepository ActivityTypes => _activityTypes.Value;
    public IDocumentRepository Documents => _documents.Value;
    public ISubmissionRepository Submissions => _submissions.Value;


    public UnitOfWork(ApplicationDbContext context,
                      Lazy<ICourseRepository> courseRepository,
                      Lazy<IModuleRepository> moduleRepository,
                      Lazy<IActivityRepository> activityRepository,
                      Lazy<IActivityTypeRepository> activityTypeRepository,
                      Lazy<IDocumentRepository> documentRepository,
                      Lazy<ISubmissionRepository> submissionRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _courses = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _modules = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        _activities = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        _activityTypes = activityTypeRepository ?? throw new ArgumentNullException(nameof(activityTypeRepository));
        _documents = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _submissions = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));

    }

    public async Task CompleteAsync() => await _context.SaveChangesAsync();
}