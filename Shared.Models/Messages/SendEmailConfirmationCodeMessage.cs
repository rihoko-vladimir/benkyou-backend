namespace Shared.Models.Messages;

public class SendEmailConfirmationCodeMessage
{
    public string EmailCode { get; init; }
    public string EmailAddress { get; init; }

    public string FirstName { get; init; }
}