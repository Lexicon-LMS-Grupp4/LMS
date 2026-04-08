namespace LMS.API.Extensions;

public static class FileStorageApplicationExtensions
{
    public static IApplicationBuilder UseDocumentFileStorage(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        return app;
    }
}