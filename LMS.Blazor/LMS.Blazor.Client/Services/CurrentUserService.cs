using LMS.Shared.DTOs.User;
using Microsoft.JSInterop;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class CurrentUserService(IApiService apiService) : ICurrentUserService
{
    private const string Base = "api/me";

    public async Task<UserDto?> GetMeAsync(CancellationToken ct = default)
        => await apiService.GetAsync<UserDto>($"{Base}", ct);
}