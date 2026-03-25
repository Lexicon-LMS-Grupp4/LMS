using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services;

public class ServerNoOpApiService(ILogger<ServerNoOpApiService> logger) : IApiService
{
    private readonly ILogger<ServerNoOpApiService> _logger = logger;

    public Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }

    public Task<T?> GetAllowNotFoundAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }

    public Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }

    public Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }

    public Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult(false);
    }
}
