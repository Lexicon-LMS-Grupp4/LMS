using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientApiService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;

        _navigationManager = navigationManager;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/proxy/{endpoint}", ct);
        HandleUnauthorized(response);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/proxy/{endpoint}", body, _jsonOptions, ct);
        HandleUnauthorized(response);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/proxy/{endpoint}", body, _jsonOptions, ct);
        HandleUnauthorized(response);
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<bool> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync($"api/proxy/{endpoint}", ct);
        HandleUnauthorized(response);
        return response.IsSuccessStatusCode;
    }

    // -------------------------
    // Private helpers
    // -------------------------

    private void HandleUnauthorized(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
        }
    }

    private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    }
}
