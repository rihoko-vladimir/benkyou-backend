namespace Benkyou.Application.Services.Common;

public interface IEmailSenderService
{
    public Task SendEmailConfirmationCodeAsync(string emailConfirmationCode, string emailReceiverAddress);
    public Task SendEmailResetLinkAsync(string emailAddress, string passwordResetToken, string name);
}