namespace Benkyou.Domain.Exceptions;

public class EmailVerificationCodeException : Exception
{
    public EmailVerificationCodeException(string? message) : base(message)
    {
    }
}