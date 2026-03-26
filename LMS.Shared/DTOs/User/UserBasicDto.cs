namespace LMS.Shared.DTOs.User
{
    /// <summary>
    /// Basic user information used across various DTOs.
    /// Represents minimal user details needed for display purposes.
    /// </summary>
    public record UserBasicDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}