using System.Collections.Generic;

namespace LMS.Shared.DTOs.Common
{
    /// <summary>
    /// Generic container for API responses.
    /// Provides a consistent structure for all API endpoints including success/error handling.
    /// </summary>
    /// <typeparam name="T">The type of data being returned in the response.</typeparam>
    public record ApiResponseDto<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public required bool Success { get; set; }

        /// <summary>
        /// The actual data being returned.
        /// Null if the operation failed or no data is available.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Optional message describing the result of the operation.
        /// Typically used for success confirmations or general error messages.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// List of specific error messages if the operation failed.
        /// Empty if Success is true.
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }
}