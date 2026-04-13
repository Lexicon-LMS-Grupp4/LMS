using LMS.Shared.DTOs.User;

namespace LMS.Blazor.Client.Services;

public interface ICurrentUserService
{
    Task<UserDto?> GetMeAsync(CancellationToken ct = default);
}