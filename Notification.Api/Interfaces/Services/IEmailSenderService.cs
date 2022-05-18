using MimeKit;
using Notification.Api.Models;

namespace Notification.Api.Interfaces.Services;

public interface IEmailSenderService
{
    public Task<Result> SendEmailAsync(MimeMessage emailMessage);
    public Task<Result> SendAccountConfirmationCodeAsync(string userName, string emailAddress, int confirmationCode);

    public Task<Result> SendForgottenPasswordResetLinkAsync(string userName, string emailAddress,
        string passwordResetToken);
}