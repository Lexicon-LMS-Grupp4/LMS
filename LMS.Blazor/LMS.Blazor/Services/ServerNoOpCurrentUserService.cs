using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs.Common;
using LMS.Shared.DTOs.Module;
using LMS.Shared.DTOs.User;
using Microsoft.Build.Framework;

namespace LMS.Blazor.Services;

public class ServerNoOpCurrentUserService(ILogger<ServerNoOpCurrentUserService> logger) : ICurrentUserService
{
    public Task<UserDto?> GetMeAsync(CancellationToken ct = default)
    {
        logger.LogWarning("ServerNoOpModuleService called for GetAllModulesAsync");
        return Task.FromResult<UserDto?>(null);
    }
}
