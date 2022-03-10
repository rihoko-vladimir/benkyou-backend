namespace Benkyou.Domain.Exceptions;

public class InvalidCardIdException : Exception
{
    public InvalidCardIdException(string? message) : base(message)
    {
    }
}