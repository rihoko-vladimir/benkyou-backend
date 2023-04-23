namespace Auth.Api.Models.Exceptions;

public class EmailNotConfirmedException : Exception
{
    public EmailNotConfirmedException(Guid userId) : base("Email for requested user is not verified")
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
}