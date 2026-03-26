using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    public ICourseRepository Courses { get; }
    public IModuleRepository Modules { get; }
    public IActivityRepository Activities { get; }
    public IDocumentRepository Documents { get; }

    public UnitOfWork(ApplicationDbContext context,
                      ICourseRepository courseRepository,
                      IModuleRepository moduleRepository,
                      IActivityRepository activityRepository,
                      IDocumentRepository documentRepository)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        Courses = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        Modules = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        Activities = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        Documents = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}