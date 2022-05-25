namespace Messages.Contracts;

public class SendEmailResetLink
{
    public string ResetToken { get; set; }
    public string EmailAddress { get; set; }
}