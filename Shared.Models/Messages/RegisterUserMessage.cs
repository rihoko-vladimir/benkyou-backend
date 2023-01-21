namespace Shared.Models.Messages;

public class RegisterUserMessage
{
    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsTermsAccepted { get; set; }
}