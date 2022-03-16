namespace Benkyou.Domain.Exceptions;

public class EmailConfirmationCodeException : Exception
{
    public EmailConfirmationCodeException(string? message) : base(message)
    {
    }
}