namespace Messages.Contracts;

public class SendEmailConfirmationCode
{
    public string EmailCode { get; set; }
    public string EmailAddress { get; set; }
}