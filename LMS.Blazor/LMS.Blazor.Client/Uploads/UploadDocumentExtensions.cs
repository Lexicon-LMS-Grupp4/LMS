using System.Net.Http.Headers;

namespace LMS.Blazor.Client.Uploads;

public static class UploadDocumentExtensions
{
    public static void AddToMultipart(
        this UploadDocumentValue value,
        MultipartFormDataContent content)
    {
        Console.WriteLine($"AddToMultipart called. HasFile={value.HasFile}, Description='{value.Description}'");

        if (value.File is null)
        {
            Console.WriteLine("AddToMultipart: value.File is null.");

            return;
        }

        var browserFile = value.File.File;
        Console.WriteLine(
            $"AddToMultipart: Name={browserFile.Name}, Size={browserFile.Size}, ContentType={browserFile.ContentType}");



        var stream = browserFile.OpenReadStream(10 * 1024 * 1024);

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(browserFile.ContentType);

        content.Add(fileContent, "File", browserFile.Name);
        content.Add(new StringContent(value.Description ?? string.Empty), "FileDescription");

        Console.WriteLine("AddToMultipart: File and FileDescription added to multipart.");

    }
}