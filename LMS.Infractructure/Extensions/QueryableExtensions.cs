using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<(IEnumerable<T> items, int totalCount)> PagedResult<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
