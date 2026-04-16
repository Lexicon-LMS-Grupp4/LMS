namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }
    IActivityRepository Activities { get; }
    IActivityTypeRepository ActivityTypes { get; }
    IDocumentRepository Documents { get; }
    ISubmissionRepository Submissions { get; }

    Task CompleteAsync();
}