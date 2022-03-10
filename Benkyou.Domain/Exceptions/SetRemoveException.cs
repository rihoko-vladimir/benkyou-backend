namespace Benkyou.Domain.Exceptions;

public class CardRemoveException : Exception
{
    public CardRemoveException(string? message) : base(message)
    {
    }
}