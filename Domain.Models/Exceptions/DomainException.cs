namespace Domain.Models.Exceptions;

public abstract class DomainException : Exception
{
    public string Title { get; }
    public int StatusCode { get; }
    private System.Collections.IDictionary? _data = new System.Collections.Hashtable();
    public new System.Collections.IDictionary Data { get=>_data; set=>_data=value; }

    protected DomainException(string message, string title, int statusCode, System.Collections.IDictionary? data=null) : base(message)
    {
        Title = title;
        StatusCode = statusCode;
        _data = data;
    }
}
