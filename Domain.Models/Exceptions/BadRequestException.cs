namespace Domain.Models.Exceptions;

public class BadRequestException : DomainException
{
    public BadRequestException(string message, string title = "Bad Request", System.Collections.IDictionary? data=null)
        : base(message, title, 400, data)
    {
    }
}
