namespace Shared.Models.Messages;

public class SendEmailResetLinkMessage
{
    public string ResetToken { get; init; }
    public string EmailAddress { get; init; }

    public string FirstName { get; init; }
}