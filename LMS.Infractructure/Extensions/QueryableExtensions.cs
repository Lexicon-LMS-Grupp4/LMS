using LMS.Shared.Request;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infractructure.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Asynchronously retrieves a single page of items from the queryable source along with the total number of items
    /// available.
    /// </summary>
    /// <remarks>The method executes the query twice: once to count the total items and once to retrieve the
    /// page. The returned collection may be empty if the page is beyond the end of the data set.</remarks>
    /// <typeparam name="T">The type of the elements in the queryable source.</typeparam>
    /// <param name="query">The source query to paginate. Must not be null.</param>
    /// <param name="page">The one-based index of the page to retrieve. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The number of items to include in each page. Must be greater than 0.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the items for the
    /// specified page and the total count of items in the source.</returns>
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
    public static async Task<(IEnumerable<T> items, int totalCount)> PagedResult<T>(
        this IQueryable<T> query,
        PagedRequestParams queryParams)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Filters the elements of an IQueryable based on a predicate if a specified condition is true.
    /// </summary>
    /// <remarks>Use this method to conditionally apply a filter to a query based on runtime logic, such as
    /// optional user input or configuration settings.</remarks>
    /// <typeparam name="T">The type of the elements in the source query.</typeparam>
    /// <param name="query">The source IQueryable to filter.</param>
    /// <param name="condition">A nullable Boolean value that determines whether the predicate should be applied. If <see langword="true"/>, the
    /// predicate is applied; if <see langword="false"/> or <see langword="null"/>, the original query is returned
    /// unfiltered.</param>
    /// <param name="predicate">An expression that represents the filter to apply to the source query if the condition is true.</param>
    /// <returns>An IQueryable that contains elements from the input sequence that satisfy the predicate if the condition is
    /// true; otherwise, the original query.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query,
                                           bool condition,
                                           Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}
