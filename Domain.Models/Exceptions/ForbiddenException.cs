namespace Domain.Models.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message, string title = "Forbidden")
        : base(message, title, 403)
    {
    }
}
