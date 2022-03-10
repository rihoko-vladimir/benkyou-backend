namespace Benkyou.Domain.Exceptions;

public class KanjiCountException : Exception
{
    public KanjiCountException(string? message) : base(message)
    {
    }
}