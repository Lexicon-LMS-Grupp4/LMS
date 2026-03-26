namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default);
}