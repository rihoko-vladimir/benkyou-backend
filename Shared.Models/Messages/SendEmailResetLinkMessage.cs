namespace Shared.Models.Messages;

public class SendEmailResetLinkMessage
{
    public string ResetToken { get; set; }
    public string EmailAddress { get; set; }

    public string FirstName { get; set; }
}