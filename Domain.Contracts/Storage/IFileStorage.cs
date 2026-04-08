using Microsoft.AspNetCore.Http;

namespace Domain.Contracts.Storage;

public interface IFileStorage
{
    Task<FileSaveResult> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string storedFileName, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string storedFileName, CancellationToken ct = default);
}