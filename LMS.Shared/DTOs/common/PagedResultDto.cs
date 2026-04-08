namespace LMS.Shared.DTOs.Common;

/// <summary>
/// Generic container for paginated results.
/// Used to return a subset of data along with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the paginated collection.</typeparam>
public record PagedResultDto<T>
{
    /// <summary>
    /// Collection of items for the current page.
    /// </summary>
    public required List<T> Items { get; init; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based index).
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total number of pages available.
    /// Calculated as ceiling(TotalCount / PageSize).
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates whether there is a previous page available.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page available.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}