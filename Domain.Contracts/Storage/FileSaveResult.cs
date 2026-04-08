namespace Domain.Contracts.Storage;

public record FileSaveResult(
    string FileName,
    long FileSize
);