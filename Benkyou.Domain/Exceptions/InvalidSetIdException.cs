namespace Benkyou.Domain.Exceptions;

public class InvalidSetIdException : Exception
{
    public InvalidSetIdException(string? message) : base(message)
    {
    }
}