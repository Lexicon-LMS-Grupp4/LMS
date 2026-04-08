using Domain.Contracts.Storage;
using LMS.Infrastructure.Options;
using LMS.Infrastructure.Storage;

namespace LMS.API.Extensions;

public static class FileStorageServiceExtensions
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        {
            var uploadRootPath = Path.Combine(
                environment.ContentRootPath,
                "AppData",
                "UploadedDocuments");

            services.Configure<FileStorageOptions>(options =>
            {
                options.UploadRootPath = uploadRootPath;
            });

            services.AddScoped<IFileStorage, LocalFileStorage>();

            return services;
        }
    }
}