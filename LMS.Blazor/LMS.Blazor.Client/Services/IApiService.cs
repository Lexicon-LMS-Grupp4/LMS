namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    /// <summary>Returns default when the API responds with 404 Not Found.</summary>
    Task<T?> GetAllowNotFoundAsync<T>(string endpoint, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default);
    Task<T?> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<T?> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
}