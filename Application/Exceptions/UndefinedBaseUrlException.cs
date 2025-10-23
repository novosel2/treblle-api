namespace Application.Exceptions;

public class UndefinedBaseUrlException : Exception
{
    public UndefinedBaseUrlException()
    {
    }

    public UndefinedBaseUrlException(string? message) : base(message)
    {
    }
}
