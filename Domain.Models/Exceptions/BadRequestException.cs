namespace Domain.Models.Exceptions;

public class BadRequestException : DomainException
{
    public BadRequestException(string message, string title = "Bad Request")
        : base(message, title, 400)
    {
    }
}
