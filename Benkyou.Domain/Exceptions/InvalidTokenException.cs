namespace Benkyou.Domain.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException(string? message) : base(message)
    {
    }
}