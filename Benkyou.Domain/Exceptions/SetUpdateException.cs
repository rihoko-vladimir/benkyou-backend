namespace Benkyou.Domain.Exceptions;

public class SetUpdateException : Exception
{
    public SetUpdateException(string? message) : base(message)
    {
    }
}