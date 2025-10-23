namespace Application.Exceptions;

public class SavingChangesFailedException : Exception
{
    public SavingChangesFailedException()
    {
    }

    public SavingChangesFailedException(string? message) : base(message)
    {
    }
}
