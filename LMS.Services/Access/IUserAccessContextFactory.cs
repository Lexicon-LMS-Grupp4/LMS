using Domain.Models.Entities;
using Service.Contracts;

namespace LMS.Services.Access;

public interface IUserAccessContextFactory
{
    Task<IUserAccessContext> CreateAsync(string userId, CancellationToken ct = default);
    Task<IUserAccessContext> CreateAsync(ApplicationUser user, CancellationToken ct = default);
}