namespace Benkyou.Domain.Exceptions;

public class CardUpdateException : Exception
{
    public CardUpdateException(string? message) : base(message)
    {
    }
}