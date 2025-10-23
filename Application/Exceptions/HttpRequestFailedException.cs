namespace Application.Exceptions;

public class HttpRequestFailedException : Exception
{
    public HttpRequestFailedException()
    {
    }

    public HttpRequestFailedException(string? message) : base(message)
    {
    }
}
