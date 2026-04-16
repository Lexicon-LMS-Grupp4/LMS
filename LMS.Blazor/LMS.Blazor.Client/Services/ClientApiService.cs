using LMS.Shared.DTOs.Common;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Models.Exceptions;

namespace LMS.Blazor.Client.Services;

public static class HttpResponseMessageExtensions{
    // https://github.com/dotnet/dotnet/blob/b0f34d51fccc69fd334253924abd8d6853fad7aa/src/runtime/src/libraries/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs#L163C13-L175C25
    public static HttpResponseMessage EnsureSuccessStatusCode(this HttpResponseMessage message, System.Collections.IDictionary? data)
	{
        if (!message.IsSuccessStatusCode)
        {
            var ex = new HttpRequestException(
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    string.IsNullOrWhiteSpace(message.ReasonPhrase) ? "Response status code does not indicate success: {0}." : "Response status code does not indicate success: {0} ({1}).",
                    (int)message.StatusCode,
                    message.ReasonPhrase),
                inner: null,
                message.StatusCode);
            // Added body Data to HttpRequestException:
            if(data != null) foreach (object key in data.Keys) ex.Data[key] = data[key];
            throw ex;
        }
        return message;
	}
}

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

    public async Task<T?> GetAllowNotFoundAsync<T>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/proxy/{endpoint}", ct);
        HandleUnauthorized(response);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/proxy/{endpoint}", body, _jsonOptions, ct);
        HandleUnauthorized(response);
        if (!response.IsSuccessStatusCode){
            // temp solution, to not get DeserializeUnableToConvertValue exception for other post calls
            if(endpoint == "api/users"){
                var json = await DeserializeAsync<BadRequestException>(response, ct);
                response.EnsureSuccessStatusCode(json?.Data);
            } else {
                response.EnsureSuccessStatusCode();
            }
        }
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

        if (!response.IsSuccessStatusCode)
        {
            ApiResponseDto<object>? apiResponse = null;

            try
            {
                apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<object>>(cancellationToken: ct);
            }
            catch
            {
                // fallback
            }

            var message =
                apiResponse?.Message ??
                apiResponse?.Errors.FirstOrDefault() ??
                $"Request failed with status {response.StatusCode}.";

            throw new Exception(message);
        }

        return true;
    }

    public async Task<T?> PostMultipartAsync<T>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync($"api/proxy/{endpoint}", content, ct);
        HandleUnauthorized(response);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine(errorBody);
        }
        response.EnsureSuccessStatusCode();
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<T?> PutMultipartAsync<T>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsync($"api/proxy/{endpoint}", content, ct);

        HandleUnauthorized(response);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine(errorBody);
        }

        response.EnsureSuccessStatusCode();

        return await DeserializeAsync<T>(response, ct);
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

    //private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    //{
    //    return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    //}
    private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            return default;

        var content = await response.Content.ReadAsStreamAsync(ct);
        if (content.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync<T>(content, _jsonOptions, ct);
    }


}
