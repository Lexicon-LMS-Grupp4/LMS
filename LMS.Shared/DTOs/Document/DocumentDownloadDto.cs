namespace LMS.Shared.DTOs.Document;

public class DocumentDownloadDto
{
    public Stream Stream { get; set; } = null!;
    public string ContentType { get; set; } = "application/octet-stream";
    public string FileDownloadName { get; set; } = "download";
}
