namespace Benkyou.Domain.Exceptions;

public class UserNotFoundExceptions : Exception
{
    public UserNotFoundExceptions(string? message) : base(message)
    {
    }
}