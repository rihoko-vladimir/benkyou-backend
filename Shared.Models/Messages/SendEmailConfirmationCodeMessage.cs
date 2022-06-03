namespace Shared.Models.Messages;

public class SendEmailConfirmationCodeMessage
{
    public string EmailCode { get; set; }
    public string EmailAddress { get; set; }
}