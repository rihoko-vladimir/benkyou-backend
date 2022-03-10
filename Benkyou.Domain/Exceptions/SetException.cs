namespace Benkyou.Domain.Exceptions;

public class SetException : Exception
{
    public SetException(string? message) : base(message)
    {
    }
}