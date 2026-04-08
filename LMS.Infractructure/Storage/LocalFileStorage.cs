using Domain.Contracts.Storage;
using LMS.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly FileStorageOptions _options;

    public LocalFileStorage(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.UploadRootPath))
            throw new InvalidOperationException("FileStorageOptions.UploadRootPath is not configured.");

        _options.UploadRootPath = Path.GetFullPath(_options.UploadRootPath);
    }

    public async Task<FileSaveResult> SaveAsync(
        IFormFile file,
        CancellationToken ct = default)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        if (string.IsNullOrWhiteSpace(file.FileName))
            throw new ArgumentException("File name is required.", nameof(file.FileName));

        Directory.CreateDirectory(_options.UploadRootPath);

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_options.UploadRootPath, storedFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fileStream, ct);

        return new FileSaveResult(
            FileName: storedFileName,
            FileSize: fileStream.Length
        );
    }

    /// <summary>
    /// Deletes a stored file if it exists.
    /// </summary>
    /// <remarks>
    /// This method returns a <see cref="Task"/> for consistency with the storage abstraction,
    /// although the underlying file deletion is performed synchronously by the local file system API.
    /// </remarks>
    /// <param name="storedFileName">The stored file name.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>
    /// <see langword="true"/> if the file existed and was deleted; otherwise, <see langword="false"/>.
    /// </returns>
    public Task<bool> DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_options.UploadRootPath, storedFileName);

        if (!File.Exists(filePath))
            return Task.FromResult(false);

        File.Delete(filePath);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Opens a stored file for reading.
    /// </summary>
    /// <param name="storedFileName">The stored file name.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A readable stream for the file.</returns>
    /// <exception cref="ArgumentException">Thrown if the file name is missing.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    public Task<Stream> OpenReadAsync(string storedFileName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storedFileName))
            throw new ArgumentException("Stored file name is required.", nameof(storedFileName));

        var filePath = Path.Combine(_options.UploadRootPath, storedFileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The requested file was not found.", storedFileName);

        Stream stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Task.FromResult(stream);
    }
}