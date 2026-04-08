using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System.Security.Claims;

[ApiController]
public abstract class LmsControllerBase : ControllerBase
{
    protected readonly IServiceManager serviceManager;
    protected string UserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("Unauthorized");

    public LmsControllerBase(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }
}